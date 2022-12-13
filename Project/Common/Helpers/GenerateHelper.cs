using Common.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class GenerateHelper
    {
        public static string CreateCodeWithLength(int length)
        {
            string code = "";
            string randomList = "0123456789abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUWXYZ";
            var random = new Random();

            for (var i = 0; i < randomList.Length; i++)
            {
                code = new string(
                    Enumerable
                    .Repeat(randomList, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray()
                );

            }
            return code;
        }

        public static string CreateCodeWithLength(int length, Func<string, bool> check)
        {
            do
            {
                var code = CreateCodeWithLength(length);
                if (!check(code)) return code;
            }
            while (true);
        }

        public static async Task<string> CreateCodeWithLengthAsync(int length, Func<string, Task<bool>> isExists)
        {
            do
            {
                var code = CreateCodeWithLength(length);
                if (!await isExists(code)) return code;
            }
            while (true);
        }

        public static async Task<string> CreateCode(string code, int length, Func<string, Task<bool>> isCheck)
        {
            if (string.IsNullOrEmpty(code))
            {
                do
                {
                    code = CreateCodeWithLength(length);
                    if (await isCheck(code)) return code;
                }
                while (true);
            }
            else
            {
                if (await isCheck(code))
                {
                    return code;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
