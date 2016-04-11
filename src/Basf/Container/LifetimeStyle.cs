
namespace Basf
{
    public enum LifetimeStyle
    {
        /// <summary>
        /// 单件
        /// </summary>
        Singleton,
        /// <summary>
        /// 线程内单件
        /// </summary>
        Thread,
        /// <summary>
        /// 瞬态
        /// </summary>
        Transient,
        /// <summary>
        /// 作用域
        /// </summary>
        Scoped,
        /// <summary>
        /// 对象池
        /// </summary>
        Pool,
        /// <summary>
        /// 请求
        /// </summary>
        Request
    }
}
