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
        public const int MANAGER = 3;
        public const int CUSTOMER = 4;
        public const int GUEST = 5;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case ADMIN:
                    return EnumResource.USER_TYPE_ADMIN;
                case EMPLOYEE:
                    return EnumResource.USER_TYPE_EMPLOYEE;
                case MANAGER:
                    return EnumResource.USER_TYPE_AGENCY;
                case CUSTOMER:
                    return EnumResource.USER_TYPE_CUSTOMER;
                case GUEST:
                    return "GUEST";
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

    public class EnumFunctions
    {
        public const int Banners = 1;
        public const int Sizes = 2;
        public const int Unit = 3;
        public const int Ingredient = 4;
        public const int Employee = 5;
        public const int Manager = 6;
        public const int Recipe_Book = 7;
        public const int Recipe = 8;
       
        public static string ToString(int? value)
        {
            switch (value)
            {
                case Banners:
                    return "Banners";
                case Sizes:
                    return "Sizes";
                case Unit:
                    return "Unit";
                case Ingredient:
                    return "Ingredient";
                case Employee:
                    return "Employee";
                case Manager:
                    return "Manager";
                case Recipe_Book:
                    return "Recipe Book";
                case Recipe:
                    return "Recipe";
                default:
                    return "";
            }
        }
    }

    public class EnumOptions
    {
        public const int FULL = 1;
        public const int VIEW = 2;
        public const int ADD = 3;
        public const int DELETE = 4;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case FULL:
                    return "Full access";
                case VIEW:
                    return "View";
                case ADD:
                    return "Insert/Update";
                case DELETE:
                    return "Delete";
                default:
                    return "";
            }
        }
    }

    public class EnumProductImageType
    {
        public const int IMAGE = 1;
        public const int BACKGROUND = 2;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case IMAGE:
                    return "Image";
                case BACKGROUND:
                    return "Background";
                default:
                    return "";
            }
        }
    }

    public class EnumIngredientData
    {
        public const int SORT_ORDER = 1;
        public const int QTY = 2;
        public const int UNIT = 3;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case SORT_ORDER:
                    return "Sort order";
                case QTY:
                    return "Qty";
                case UNIT:
                    return "Unit";
                default:
                    return "";
            }
        }
    }

    public class EnumStepData
    {
        public const int SORT_ORDER = 1;
        public const int DESCRIPTION = 2;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case SORT_ORDER:
                    return "Sort order";
                case DESCRIPTION:
                    return "Description";
                default:
                    return "";
            }
        }
    }

    public class EnumSize
    {
        public const int REGULAR = 13;
        public const int LARGE = 14;

        public static string ToString(int? value)
        {
            switch (value)
            {
                case REGULAR:
                    return "Regular";
                case LARGE:
                    return "Large";
                default:
                    return "";
            }
        }
    }
}
