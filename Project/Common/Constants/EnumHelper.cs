namespace Common.Constants
{
    public static class EnumHelper
    {
        public static string ToName(this ErrorCode role)
        {
            switch (role)
            {
                case ErrorCode.PASS: return "00";
                case ErrorCode.ACTION_NOTFOUND: return "01";
                case ErrorCode.NOT_PERMISSION: return "02";
                default: return "";
            }
        }

        public static string ToName(this RolePermission role)
        {
            switch (role)
            {
                case RolePermission.SYSTEM_ADMIN: return "SYS_ADM";
                case RolePermission.ADMIN: return "ADM";
                case RolePermission.MANAGER: return "MNG";
                case RolePermission.SALE_MANAGER: return "SALE_MNG";
                case RolePermission.SALE: return "SALE";
                case RolePermission.CUSTOMER: return "CUS";
                default: return "";
            }
        }

        public static string ToAction(this RoleActionEnum role)
        {
            switch (role)
            {
                case RoleActionEnum.CREATE: return "CREATE";
                case RoleActionEnum.DELETE: return "DELETE";
                case RoleActionEnum.LIST: return "LIST";
                case RoleActionEnum.LIST_COMPLAIN: return "LIST_COMPLAIN";
                case RoleActionEnum.LIST_PREPARE: return "LIST_PREPARE";
                case RoleActionEnum.READ: return "READ";
                case RoleActionEnum.UPDATE: return "UPDATE";
                case RoleActionEnum.UPDATE_STATUS: return "UPDATE_STATUS";
                case RoleActionEnum.LIST_CATE: return "LIST_CATE";
                default: return "";
            }
        }

        public static string ToController(this Controller role)
        {
            switch (role)
            {
                case Controller.CATEGORY: return "CATEGORY";
                case Controller.COMPLAIN: return "COMPLAIN";
                case Controller.ORDER: return "ORDER";
                case Controller.PACKAGE: return "PACKAGE";
                case Controller.PRODUCT: return "PRODUCT";
                case Controller.ROLE: return "ROLE";
                case Controller.UNIT: return "UNIT";
                case Controller.USER: return "USER";
                default: return "";
            }
        }
    }
    public static class ImageHelper
    {
        public static string GetImage(this string image)
        {
            if (string.IsNullOrEmpty(image))
            {
                return EnumImageDefault.LOGO.Replace("~/", "");
            }
            else
            {
                return image.Replace("~/", "");
            }
        }
    }

    #region System Role

    public enum ErrorCode
    {
        NOT_PERMISSION,
        ACTION_NOTFOUND,
        PASS,
    }

    public enum RolePermission
    {
        SYSTEM_ADMIN,
        ADMIN,
        MANAGER,
        SALE_MANAGER,
        SALE,
        CUSTOMER,
    }

    public enum RoleActionEnum
    {
        LIST,
        READ,
        CREATE,
        UPDATE,
        DELETE,
        UPDATE_STATUS,
        LIST_COMPLAIN,
        LIST_PREPARE,
        LIST_CATE,
    }

    public enum Controller
    {
        USER,
        UNIT,
        ROLE,
        PRODUCT,
        CATEGORY,
        PACKAGE,
        ORDER,
        COMPLAIN,
    }

    public enum KeyDict
    {
        ACTION,
        CONTROLLER,
        ROLE_ACTION,
    }

    #endregion

    public enum LoginType
    {
        Default,
        Facebook,
        Google,
        Apple
    }
}
