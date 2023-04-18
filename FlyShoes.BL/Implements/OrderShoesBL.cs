using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.DAL.Interfaces;
using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Base
{
    public class OrderShoesBL : BaseBL<OrderShoes>,IOrderShoesBL
    {
        IFirestoreService _firstoreService;
        public OrderShoesBL(IDatabaseService databaseService,IFirestoreService firestoreService) : base(databaseService)
        {
            _firstoreService = firestoreService;
        }

        public async Task<List<OrderShoes>> GetOrdersByUser()
        {
            var commandGetOrder = "Select * from OrderShoes os WHERE os.UserID = @UserID;";
            var param = new Dictionary<string, object>()
            {
                {"@UserID",_dataBaseService.CurrentUser.UserID }
            };

            return await _dataBaseService.QueryUsingCommanTextAsync<OrderShoes>(commandGetOrder,param);
        }

        public async Task<ServiceResponse> Order(int paymentType, OrderShoes orderShoes)
        {
            var result = new ServiceResponse();

            /// Thực hiện cập nhật số lượng hàng trước khi tạo đơn
            var index = 0;
            var param = new Dictionary<string, object>();
            StringBuilder updateQuantity = new StringBuilder();
            var connection = _dataBaseService.GetDbConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();
            foreach(var orderDetail in orderShoes.OrderDetails)
            {
                param.Add($"@Quantity_{index}", orderDetail.Quantity);
                param.Add($"@ShoesDetailID_{index}", orderDetail.ShoesDetailID);
                updateQuantity.Append($"UPDATE ShoesDetail sd SET sd.Quantity = sd.Quantity - @Quantity_{index} WHERE sd.ShoesDetailID = @ShoesDetailID_{index} AND (sd.Quantity - @Quantity_{index} > 0 OR sd.Quantity - @Quantity_{index} = 0);");
                index++;
            }

            var resUpdateQuantity = await _dataBaseService.ExecuteUsingCommandTextAsync(updateQuantity.ToString(), param,transaction,connection);

            if(resUpdateQuantity != orderShoes.OrderDetails.Count)
            {
                transaction.Rollback();
                transaction.Dispose();
                connection.Close();
                result.Success = false;
                result.ValidateInfo.Add(new ValidateResult()
                {
                    ErrorCode = ErrorCodeEnum.ProductNotEnough
                });

                return result;
            }

            /// Nếu có voucher thì tạo voucherused
            /// Giảm số lượng voucher
            var voucherIDs = orderShoes.OrderDetails.Where(orderDetail => orderDetail.VoucherID != null)?.Select(orderDetail => orderDetail.VoucherID)?.ToList();
            if(voucherIDs != null && voucherIDs.Count > 0)
            {
                var paramUpdateVoucher = new Dictionary<string, object>()
                {
                    {"@DateNow",System.DateTime.Now }
                };
                var updateVoucher = "UPDATE Voucher v SET v.Quantity = v.Quantity - 1 WHERE v.VoucherID IN ({0}) AND (v.Quantity - 1 > 0 OR v.Quantity - 1 = 0) AND v.EndDate > @DateNow AND v.IsActive IS TRUE;";

                var resUpdateVoucher = await _dataBaseService.ExecuteUsingCommandTextAsync(string.Format(updateVoucher, string.Join(",", voucherIDs)), paramUpdateVoucher, transaction,connection);
                if(resUpdateVoucher != voucherIDs.Count)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    result.Success = false;
                    result.ValidateInfo.Add(new ValidateResult()
                    {
                        ErrorCode = ErrorCodeEnum.VoucherNotEnough
                    });

                    return result;
                }

                StringBuilder insertVoucherUsed = new StringBuilder();
                var indexInsert = 0;
                var paramInsert = new Dictionary<string, object>()
                {
                    {"@DateNow",System.DateTime.Now },
                    {"@UserID",_dataBaseService.CurrentUser.UserID },
                    {"@UserName",_dataBaseService.CurrentUser.FullName },
                };
                foreach(var idVoucher in voucherIDs)
                {
                    insertVoucherUsed.Append($"INSERT INTO VoucherUsed (VoucherID,UserID,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy) VALUES (@VoucherID_{indexInsert},@UserID,@DateNow,@DateNow,@UserName,@UserName);");
                    paramInsert.Add($"@VoucherID_{indexInsert}", idVoucher);
                    indexInsert++;
                }
                _dataBaseService.ExecuteUsingCommandText(insertVoucherUsed.ToString(), paramInsert, transaction,connection);
            }

            /// Cập nhật số tiền bỏ ra cho user
            var commandForUser = "UPDATE User u SET u.AmountSpent = u.AmountSpent + @TotalBill WHERE u.UserID = @UserID;";
            var paramUpdateUser = new Dictionary<string, object>()
            {
                {"@TotalBill",orderShoes.TotalBill },
                {"@UserID",_dataBaseService.CurrentUser.UserID }
            };
            await _dataBaseService.ExecuteUsingCommandTextAsync(commandForUser, paramUpdateUser,transaction,connection);

            BeforeSave(orderShoes);

            /// Lưu Order   
            orderShoes.UserID = _dataBaseService.CurrentUser.UserID;
            var id = DoInsert(orderShoes,connection,transaction);
            orderShoes.OrderDetails.ForEach(orderDetail => orderDetail.OrderID = id);
            DoInsertMulti(orderShoes.OrderDetails, connection, transaction);

            /// Nếu là sản phẩm trong giở hàng xóa cartdetail
            if(paymentType == (int)PaymentType.Full)
            {
                var deleteCartDetail = "DELETE FROM CartDetail WHERE UserID = @UserID AND ShoesID IN ({0});";
                var shoesIDs = orderShoes.OrderDetails.Select(orderDetail => orderDetail.ShoesID).ToList();
                var paramDelete = new Dictionary<string, object>()
                {
                    {"@UserID",_dataBaseService.CurrentUser.UserID }
                };

                await _dataBaseService.ExecuteUsingCommandTextAsync(string.Format(deleteCartDetail, string.Join(",", shoesIDs)), paramDelete,transaction,connection);
            }

            result.Success = true;
            transaction.Commit();
            transaction.Dispose();
            connection.Close();

            var notification = new Notification()
            {
                UserID = _dataBaseService.CurrentUser.UserID,
                Message = "Đơn hàng của bạn đã được tạo và đang chờ duyệt, cảm ơn bạn đã ủng hộ shop ❤️"
            };
            _ = _firstoreService.PushNotification(notification).ConfigureAwait(false);

            return result;
        }
    }
}
