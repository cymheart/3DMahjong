using System.Collections.Generic;

namespace Net
{
    public class ShardRes
    {
        Dictionary<string, HttpRequestMethod> httpReqMethodMap = new Dictionary<string, HttpRequestMethod>();
        Dictionary<HttpRequestMethod, string> httpReqMethodStrMap = new Dictionary<HttpRequestMethod, string>();

        // 定义一个静态变量来保存类的实例
        private static ShardRes uniqueInstance;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // 定义私有构造函数，使外界不能创建该类实例
        private ShardRes()
        {
            httpReqMethodMap["GET"] = HttpRequestMethod.HTTP_METHOD_GET;
            httpReqMethodMap["POST"] = HttpRequestMethod.HTTP_METHOD_POST;
            httpReqMethodMap["PUT"] = HttpRequestMethod.HTTP_METHOD_PUT;
            httpReqMethodMap["DELETE"] = HttpRequestMethod.HTTP_METHOD_DELETE;
            httpReqMethodMap["HEAD"] = HttpRequestMethod.HTTP_METHOD_HEAD;
            httpReqMethodMap["TRACE"] = HttpRequestMethod.HTTP_METHOD_TRACE;
            httpReqMethodMap["CONNECT"] = HttpRequestMethod.HTTP_METHOD_CONNECT;
            httpReqMethodMap["PATCH"] = HttpRequestMethod.HTTP_METHOD_PATCH;
            httpReqMethodMap["OPTIONS"] = HttpRequestMethod.HTTP_METHOD_OPTIONS;


            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_GET] = "GET";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_POST] = "POST";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_PUT] = "PUT";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_DELETE] = "DELETE";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_HEAD] = "HEAD";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_TRACE] = "TRACE";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_CONNECT] = "CONNECT";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_PATCH] = "PATCH";
            httpReqMethodStrMap[HttpRequestMethod.HTTP_METHOD_OPTIONS] = "OPTIONS";
        }

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static ShardRes GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new ShardRes();
                    }
                }
            }
            return uniqueInstance;
        }

        public HttpRequestMethod GetHttpRequestMethod(string key)
        {
            string k = key.ToUpper();

            if (httpReqMethodMap.ContainsKey(k))
            {
                HttpRequestMethod method = httpReqMethodMap[k];
                return method;
            }
            return HttpRequestMethod.HTTP_METHOD_UNKNOWN;
        }


        public string GetHttpRequestMethodStr(HttpRequestMethod reqMethod)
        {
            if (httpReqMethodStrMap.ContainsKey(reqMethod))
            {
                return httpReqMethodStrMap[reqMethod];
            }

            return "GET";
        }
    }
}
