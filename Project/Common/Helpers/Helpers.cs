using Common.Constants;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Common.Helpers
{
    public static class Helpers
    {
        public static string[] uploadFile(this HttpPostedFileBase file, string rootPath, string filePath, string fileName = "")
        {
            string[] array = new string[6]
            {
                null,
                null,
                null,
                null,
                "false",
                null
            };
            if (file != null)
            {
                string text = Path.GetExtension(file.FileName).ToLower();
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string text2 = (fileName.Trim() == "") ? Guid.NewGuid().ToString() : fileName;
                fileName = text2 + text;
                filePath = Path.Combine(filePath, fileName);
                file.SaveAs(filePath);
                array[0] = text2;
                array[1] = rootPath + "/" + fileName;
                array[2] = text;
                array[3] = rootPath;
                FileInfo fileInfo = new FileInfo(filePath);
                array[4] = "true";
                array[5] = fileInfo.Length.ToString();
            }

            return array;
        }

        public static string[] uploadFileImageCompress(this HttpPostedFileBase file, string rootPath, string filePath, string fileName = "")
        {
            string[] array = new string[6]
            {
                null,
                null,
                null,
                null,
                "false",
                null
            };
            if (file != null)
            {
                Bitmap bitmap = new Bitmap(file.InputStream);
                ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder quality = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(quality, 50L);
                encoderParameters.Param[0] = encoderParameter;
                bitmap.Save(filePath, encoder, encoderParameters);
                string text = Path.GetExtension(file.FileName).ToLower();
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string text2 = (fileName.Trim() == "") ? Guid.NewGuid().ToString() : fileName;
                fileName = text2 + text;
                filePath = Path.Combine(filePath, fileName);
                file.SaveAs(filePath);
                array[0] = text2;
                array[1] = rootPath + "/" + fileName;
                array[2] = text;
                array[3] = rootPath;
                FileInfo fileInfo = new FileInfo(filePath);
                array[4] = "true";
                array[5] = fileInfo.Length.ToString();
            }

            return array;
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo[] array = imageDecoders;
            foreach (ImageCodecInfo imageCodecInfo in array)
            {
                if (imageCodecInfo.FormatID == format.Guid)
                {
                    return imageCodecInfo;
                }
            }

            return null;
        }


        public static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    text = text.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return text;
        }

        public static bool CheckPhone(string phone, string regionsCode = "VN")
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var phoneNumber = phoneNumberUtil.Parse(phone, regionsCode);
            return phoneNumberUtil.IsValidNumberForRegion(phoneNumber, regionsCode);
        }
        public static string GetPhoneByType(string phone, int type, string regionsCode = "VN")
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var phoneNumber = phoneNumberUtil.Parse(phone, regionsCode);
            var check = phoneNumberUtil.IsValidNumberForRegion(phoneNumber, regionsCode);
            if (check)
            {
                return type == EnumGetPhoneData.NORMAL ? "0" + phoneNumber.NationalNumber.ToString() : phoneNumber.CountryCode.ToString() + phoneNumber.NationalNumber.ToString();
            }
            return "";

        }

        public static string RemoveSpecialCharacters(this string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "[^a-zA-Z0-9 ]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        public static string RemoveSpace(this string text)
        {
            while (text.IndexOf("  ") != -1)
            {
                text = text.Replace("  ", " ");
            }
            return text;
        }

        public static string CreateCodeNumberWithLength(int length)
        {
            string result = "";
            var random = new Random();
            int minimumRange = 0;
            int maximumRange = 9;
            for (int i = 0; i < length; i++)
            {
                int randomNumber = random.Next(minimumRange, maximumRange);
                result += randomNumber;
            }
            return result;
        }

        public static string CreateCode9Char()
        {
            string result = "";
            var random = new Random();
            int minimumRange = 0;
            int maximumRange = 9;
            for (int i = 0; i < 9; i++)
            {
                int randomNumber = random.Next(minimumRange, maximumRange);
                result += randomNumber;
            }
            return result;
        }

        public static string CreateCode6Char()
        {
            string result = "";
            var random = new Random();
            int minimumRange = 0;
            int maximumRange = 9;
            for (int i = 0; i < 6; i++)
            {
                int randomNumber = random.Next(minimumRange, maximumRange);
                result += randomNumber;
            }
            return result;
        }

        public static string CreateCode4Char()
        {
            string result = "";
            var random = new Random();
            int minimumRange = 0;
            int maximumRange = 9;
            for (int i = 0; i < 4; i++)
            {
                int randomNumber = random.Next(minimumRange, maximumRange);
                result += randomNumber;
            }
            return result;
        }

        public static string CreateCodeTimeStamp()
        {
            var timeStamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            return timeStamp.ToString();
        }

        public static DateTime? ToDateTime(this string text)
        {
            try
            {
                List<int> iText = text.Split('/').Select(int.Parse).ToList();
                return new DateTime(iText[2], iText[1], iText[0]);
            }
            catch
            {
                return null;
            }

        }

        public static DateTime? ToFullDateTime(string text)
        {
            try
            {
                string time = text.Split(' ')[0];
                string date = text.Split(' ')[1];
                List<int> iTextTime = time.Split(':').Select(int.Parse).ToList();
                List<int> iTextDate = date.Split('/').Select(int.Parse).ToList();
                return new DateTime(iTextDate[2], iTextDate[1], iTextDate[0], iTextTime[0], iTextTime[1], 0);
            }
            catch
            {
                return null;
            }

        }

        public static string GenarateRandomString()
        {
            string result = "";
            string randomList = "0123456789abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUWXYZ";
            //var stringChars = new char[1];
            var random = new Random();

            for (var i = 0; i < randomList.Length; i++)
            {
                result = new string(Enumerable.Repeat(randomList, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            }
            return result;
        }
        public static string GenarateRandomStringWithLength(int? length = 7)
        {
            string result = "";
            string randomList = "0123456789abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUWXYZ";
            //var stringChars = new char[1];
            var random = new Random();

            for (var i = 0; i < randomList.Length; i++)
            {
                result = new string(Enumerable.Repeat(randomList, length.Value)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            }
            return result;
        }


        public static void WriteLog(string pathRoot, params string[] content)
        {
            try
            {
                DateTime dt = DateTime.Now;

                string directoryPath = Path.Combine(pathRoot, dt.Year.ToString(), dt.Month.ToString());
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, "Log_" + dt.ToString("yyyyMMdd") + ".txt");
                StreamWriter sw;
                if (!File.Exists(filePath))
                {
                    sw = File.CreateText(filePath);
                }
                else
                {
                    sw = File.AppendText(filePath);
                    sw.WriteLine();
                }

                sw.WriteLine(dt.ToString("HH:mm:ss"));
                foreach (string item in content)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string Params(this string result, params string[] suffix)
        {
            string mes = "";
            if (suffix.Length > 0)
            {
                for (var i = 0; i < suffix.Length; i++)
                {
                    mes = suffix[i] + " ";
                    result = result.Replace("{" + i + "}", mes);
                }
            }
            else
            {
                result = result.Replace("{0}", "Thao tác");
            }


            return result.ToLower().Trim().FirstCharToUpper();
        }
        public static string FirstCharToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static MailMessage SendMail(string sender, string reveiver, string senderPass, string title, string content)
        {
            try
            {
                var mail = new MailMessage(sender, reveiver, title, content);
                var smtp = new SmtpClient
                {
                    Host = "mail.shopdi.co",
                    Port = 25,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(sender, senderPass),
                };
                var view = AlternateView.CreateAlternateViewFromString(content, null, "text/html");
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(view);
                smtp.Timeout = 999999999;
                smtp.Send(mail);

                return mail;
            }
            catch
            {
                return null;
            }
        }

        public static MailMessage SendMailCC(string sender, string reveiver, string senderPass, string title, string content, string mailCC = "")
        {
            try
            {
                var mail = new MailMessage(sender, reveiver, title, content);
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(sender, senderPass),
                };
                var view = AlternateView.CreateAlternateViewFromString(content, null, "text/html");
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(view);
                var copy = new MailAddress(mailCC);
                mail.CC.Add(copy);
                smtp.Timeout = 999999999;
                smtp.Send(mail);

                return mail;
            }
            catch
            {
                return null;
            }
        }

        public static IQueryable<TEntity> SortOrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, int desc)
        {
            string command = desc == EnumSortBy.DES ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }


        public static string ChangeVersion(string version)
        {
            try
            {
                var newVersion = "";
                var arr = version.Split('.').ToList();
                var i1 = int.Parse(arr[0]);
                var i2 = int.Parse(arr[1]);
                var i3 = int.Parse(arr[2]);

                if (i3 + 1 >= 10)
                {
                    i3 = 0;

                    if (i2 + 1 >= 10)
                    {
                        i2 = 0;
                        i1 += 1;
                    }
                    else
                    {
                        i2 += 1;

                    }

                }
                else
                {
                    i3 += 1;
                }
                newVersion = i1.ToString() + "." + i2.ToString() + "." + i3.ToString();
                return newVersion;
            }
            catch
            {
                return version;
            }
        }

        public static string ConvertCurrency(Int64 money)
        {
            string data = String.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c}", Convert.ToInt32(money));
            var index = data.IndexOf(',');
            var last = data.Split(',')[1].Split(' ')[1];
            string result = data.Replace(",", "").Substring(0, index) + " " + last;
            return result;
        }

        public static TimeSpan ToTimeSpan(this string time)
        {
            return TimeSpan.Parse(time);
        }
        public static bool CheckTimeSpan(this DateTime date, TimeSpan timeStart, TimeSpan timeEnd)
        {
            if (date == null)
            {
                return false;
            }
            string time = date.Hour + 1 + ":" + date.Minute;
            TimeSpan timeCheck = TimeSpan.Parse(time);
            if (timeCheck >= timeStart && timeCheck <= timeEnd)
            {
                return true;
            }
            return false;


        }

        public static long CheckTimeSpanRemain(this DateTime date, TimeSpan timeEnd, TimeSpan timeStart)
        {
            if (date == null)
            {
                return 0;
            }
            string time = date.Hour + ":" + date.Minute;
            DateTime timeCheckEnd = DateTime.Today;
            timeCheckEnd.AddHours(timeEnd.Hours);
            timeCheckEnd.AddMinutes(timeEnd.Minutes);

            DateTime timeCheckStart = DateTime.Today;
            timeCheckStart.AddHours(timeStart.Hours);
            timeCheckStart.AddMinutes(timeStart.Minutes);
            long totalCompareTimeStart = Convert.ToInt64((date - timeCheckStart).TotalSeconds);
            long totalSecond = Convert.ToInt64((timeCheckEnd - date).TotalSeconds);
            if (totalCompareTimeStart > 0 && totalSecond > 0)
            {
                return totalSecond;
            }
            else
            {
                return 0;
            }



        }

        #region OTP firebase
        public static Dictionary<string, dynamic> VerifyRefreshToken(this string refresh_token)
        {
            //var grant_type = "refresh_token";
            //var api_key = "AIzaSyDz2YLCgHKJAO6pEtXazJPnF74jtG4JEWE";
            //var parms = new SortedList<string, string>();
            //parms["key"] = api_key;
            //parms["grant_type"] = grant_type;
            //parms["refresh_token"] = refresh_token;

            //string post_data = "";
            //foreach (KeyValuePair<string, string> parm in parms)
            //{
            //    if (post_data.Length > 0) { post_data += "&"; }
            //    post_data += parm.Key + "=" + Uri.EscapeDataString(parm.Value);
            //}
            //// do the post:
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //System.Net.WebClient cl = new System.Net.WebClient();
            //cl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //cl.Encoding = Encoding.UTF8;

            var ret = new Dictionary<string, dynamic>();
            try
            {
                #region Xác thực thời hạn access tokenkey của apple
                JObject dataInfo = new JObject();
                var jwtSecurityToken = new JwtSecurityToken(refresh_token);
                foreach (var item in jwtSecurityToken.Claims)
                    dataInfo[item.Type] = item.Value;
                #endregion

                //string url = string.Format("https://securetoken.googleapis.com/v1/token", ConfigurationManager.AppSettings["BaseAPI"]);
                //string resp = cl.UploadString(url, post_data);
                //ret = decoder.Deserialize<Dictionary<string, dynamic>>(resp);
                ////if(ret["success"] != null && ret["success"] == false)
                ////    return Json(new { status = 0, data = ret, msg = "Error!" }, JsonRequestBehavior.AllowGet);

                if (dataInfo["phone_number"] != null)
                {
                    ret["success"] = true;
                    ret["data"] = dataInfo;
                }
                else
                {
                    ret["success"] = false;
                }

            }
            catch (WebException e)
            {
                ret["success"] = false;
            }
            catch (Exception e)
            {
                ret["success"] = false;
            }
            return ret;
        }
        #endregion

        public static DateTimeOffset ToDateTimeOffset(this DateTime date)
        {
            DateTimeOffset utcTime1 = new DateTimeOffset(date,
                          TimeZoneInfo.Local.GetUtcOffset(date));
            return utcTime1;
        }


    }
}
