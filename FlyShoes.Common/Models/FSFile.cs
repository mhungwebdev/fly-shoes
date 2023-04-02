using FlyShoes.Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class FSFile
    {
        public Guid FileID { get; set; }

        public IFormFile File { get; set; }

        public FSFile(IFormFile file)
        {
            FileID = Guid.NewGuid();
            File = file;
        }
    }
}
