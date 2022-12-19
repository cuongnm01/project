using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;

namespace Project.Model.Respone
{
    public class CxResponse : ErrorResult
    {
        public override bool isSuccess => string.IsNullOrEmpty(code) || code == "200";
        public override string code { get; set; }
        public override string message { get; set; }
        public override string version { get; set; } = new AppDbContext().AppConfigs.FirstOrDefault().VersionMaster.ToString() ?? "";
        public CxResponse()
        {
        }

        public CxResponse(string message)
        {
            this.message = message;
        }
        public CxResponse(string code, string message)
        {
            this.code = code;
            this.message = message;
        }

    }

    public class CxResponse<T> : CxResponse
    {
        public T data { get; set; }

        public CxResponse(string message) : base(message)
        {
            this.code = ((int)HttpStatusCode.BadRequest).ToString();
        }

        public CxResponse(T data, string message = "Thành công")
        {
            this.message = message;
            this.code = ((int)HttpStatusCode.OK).ToString();
            this.data = data;
        }
    }

    public class Pagination<T>
    {
        public List<T> data { get; set; }

        public int totalRecord { get; set; }
        public int totalPage { get; set; }
        public int pageSize { get; set; }
        public int page { get; set; }
        public Pagination(List<T> data, int page, int pageSize)
        {
            this.totalRecord = data.Count();
            this.page = page;
            this.pageSize = pageSize;
            this.totalPage = (totalRecord + pageSize - 1) / pageSize;
            this.data = data.Skip((page > 0 ? page - 1 : page) * pageSize).Take(pageSize).ToList();
        }
    }
}
