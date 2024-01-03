using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Cors;


[assembly: OwinStartup(typeof(websocket.Startup1))]

namespace websocket
{
    public class Startup1
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    // 如需如何設定應用程式的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=316888
        //}

        public void Configuration(IAppBuilder app)
        {
            // 設定跨域資源共用
            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true
            };

            // 如果需要限制特定來源，使用 WithOrigins 指定來源網址
            corsPolicy.Origins.Add("http://example.com");

            // 設定 CORS 中介軟體
            app.UseCors(new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            });

            // 其他 SignalR 配置...
            app.MapSignalR();
        }
    }
}
