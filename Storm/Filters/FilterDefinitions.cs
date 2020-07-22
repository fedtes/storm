using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Filters
{
    public class AndFilter : MultiFilter
    {

    }

    public class OrFilter : MultiFilter
    {

    }

    public class EqualToFilter : MonoFilter { }
    public class NotEqualToFilter : MonoFilter { }
    public class GreaterToFilter : MonoFilter { }
    public class GreaterOrEqualToFilter : MonoFilter { }
    public class LessToFilter : MonoFilter { }
    public class LessOrEqualToFilter : MonoFilter { }
    public class LikeFilter : MonoFilter { }
    public class NotLikeFilter : MonoFilter { }
    public class InFilter : MonoFilter { }
    public class NotInFilter : MonoFilter { }
    public class IsNullFilter : MonoFilter { }
    public class IsNotNullFilter : MonoFilter { }

}
