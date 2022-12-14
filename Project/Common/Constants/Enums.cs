using Common.Resources;

namespace Common.Constants
{
    public static class EnumSortBy
    {
        public const int ACS = 1;
        public const int DES = 2;

        public static string toString(int? value)
        {
            switch (value)
            {
                case ACS: return "Thấp đến cao";
                case DES: return "Cao đến thấp";
                default: return "";
            }
        }
    }
    public static class EnumStatusVoucher
    {
        public const int INACTIVE = 2;
        public const int ACTIVE = 1;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case INACTIVE:
                    return "Chưa sử dụng";
                case ACTIVE:
                    return "Đã sử dụng";
                default:
                    return "";
            }
        }
    }

    public static class EnumStatus
    {
        public const int INACTIVE = 2;
        public const int ACTIVE = 1;
        public const int FINISH = 3;
        public const int CANCEL = 4;
        public const int DELETE = 5;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case INACTIVE:
                    return EnumResource.STATUS_INACTIVE;
                case ACTIVE:
                    return EnumResource.STATUS_ACTIVE;
                case FINISH:
                    return EnumResource.STATUS_FINISH;
                case CANCEL:
                    return EnumResource.STATUS_CANCEL;
                case DELETE:
                    return "Chờ xoá";
                default:
                    return "";
            }
        }
    }
    public static class EnumUserType
    {
        public const int ADMIN = 1;
        public const int EMPLOYEE = 2;
        public const int AGENCY = 3;
        public const int CUSTOMER = 4;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case ADMIN:
                    return EnumResource.USER_TYPE_ADMIN;
                case EMPLOYEE:
                    return EnumResource.USER_TYPE_EMPLOYEE;
                case AGENCY:
                    return EnumResource.USER_TYPE_AGENCY;
                case CUSTOMER:
                    return EnumResource.USER_TYPE_CUSTOMER;
                default:
                    return "";
            }
        }
    }
    public static class EnumNotificationType
    {
        public const int INTRO = 1;
        public const int SYSTEM = 2;
        public const int PROMOTION = 3;
        public const int ORDER = 4;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case INTRO:
                    return EnumResource.NOTIFICATION_TYPE_INTRO;
                case SYSTEM:
                    return EnumResource.NOTIFICATION_TYPE_SYSTEM;
                case PROMOTION:
                    return EnumResource.NOTIFICATION_TYPE_PROMOTION;
                case ORDER:
                    return EnumResource.NOTIFICATION_TYPE_ORDER;
                default:
                    return "";
            }
        }
    }

    public static class EnumPromotionDiscountType
    {
        public const int VOUCHER = 1;
        public const int PRODUCT = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case VOUCHER:
                    return EnumResource.PROMOTION_DISCOUNT_TYPE_VOUCHER;
                case PRODUCT:
                    return EnumResource.PROMOTION_DISCOUNT_TYPE_PRODUCT;
                default:
                    return "";
            }
        }
    }
    public static class EnumPromotionType
    {
        public const int PRICE = 1;
        public const int PERCENT = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case PRICE:
                    return EnumResource.PROMOTION_TYPE_PRICE;
                case PERCENT:
                    return EnumResource.PROMOTION_TYPE_PERCENT;
                default:
                    return "";
            }
        }
    }
    public static class EnumPromotionConditionType
    {
        public const int ORDER = 1;
        public const int SHIPPING = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case ORDER:
                    return EnumResource.PROMOTION_CONDITION_TYPE_ORDER;
                case SHIPPING:
                    return EnumResource.PROMOTION_CONDITION_TYPE_SHIPPING;
                default:
                    return "";
            }
        }
    }

    public static class EnumNewType
    {
        public const int NEWS = 1;
        public const int PROMOTION = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case NEWS:
                    return EnumResource.NEW_TYPE_NEWS;
                case PROMOTION:
                    return EnumResource.NEW_TYPE_PROMOTION;
                default:
                    return "";
            }
        }
    }
    public static class EnumMediaRefernalType
    {
        public const int PRODUCT = 1;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case PRODUCT:
                    return Message.F_PRODUCT;
                default:
                    return "";
            }
        }
    }
    public static class EnumGetPhoneData
    {
        public const int NORMAL = 1;
        public const int REGION = 2;

    }
    public static class EnumSearchProduct
    {
        public const int ALL = 1;
        public const int NEW = 2;
        public const int CATEGORIES = 3;
        public const int PROMOTION = 4;

    }
    public static class EnumImageDefault
    {
        public const string LOGO = "~/Files/images/default/user_empty.jpg";
        public const string AVATAR_EMPTY = "~/Files/images/default/user_empty.jpg";
        public const string PRODUCT_EMPTY = "~/Files/images/default/default_large.png";
        public const string NEWS_EMPTY = "~/Files/images/default/default_large.png";
        public const string ICON_GIEO_TRONG = "~/Files/git/gieo-trong.gif";
        public const string ICON_RA_HOA = "~/Files/git/ra-hoa.gif";
        public const string ICON_KET_QUA = "~/Files/git/ra-qua.gif";
        public const string ICON_THU_HOACH = "~/Files/git/thu-hoach.gif";
        public const string COLOR_GIEO_TRONG = "#cce9f2";
        public const string COLOR_RA_HOA = "#f29366";
        public const string COLOR_KET_QUA = "#a6ce3a";
        public const string COLOR_THU_HOACH = "#E89AFF";
        public const string COLOR_DEFAULT = "#FFFFFF";
        public const int TOTAL_MAX = int.MaxValue;


    }
}
