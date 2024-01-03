using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace websocket
{
    public class SyscomHub : Hub
    {
        //記錄使用者及文件資訊
        //1.Sid：儲存SignalR自動產生的唯一識別碼 
        //2.Name：儲存使用者於連線後自訂的名字 
        //3.ConnectedTime：記錄使用者於何時連線的時間
        public class UserData
        {
            public string Sid { get; set; }
            public string Name { get; set; }
            public string ConnectedTime { get; set; }
        }

        //記錄使用者及文件資訊
        //1.lastEditId：記錄最後編輯使用者的Id 
        //2.lastEditTime：記錄最後編輯的時間 
        //3.lastEditName：記錄最後編輯使用者的名字 
        //4.content：文件的內容 而上線清單利用Dictionary來儲存Id及使用者名稱
        public class DocData
        {
            public string lastEditId { get; set; }
            public string lastEditTime { get; set; }
            public string lastEditName { get; set; }
            public string content { get; set; }
        }

        //儲存Id及使用者名稱
        public static ConcurrentDictionary<string, UserData> ConcurrentedUserList = new ConcurrentDictionary<string, UserData>();

        //暫存文件的內容
        public static DocData docData = new DocData();

        public void Hello()
        {
            Clients.All.hello();
        }

        //Server端的Function提供Client呼叫
        //使用者連線到SignalR Hub時呼叫此Function
        public void onJoinRoom(UserData data)
        {
            //抓取使用者連線上來的時間
            data.ConnectedTime = DateTime.Now.ToString("f");

            //將使用者資訊加入剛剛定義的上線清單Dictionary裡
            ConcurrentedUserList.TryAdd(Context.ConnectionId, data);

            //呼叫Client Code，呼叫所有使用者
            Clients.All.getOnlineList(ConcurrentedUserList);
            Clients.All.updateDocContent(docData);
        }

        ////使用者離線時
        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    UserData Value;
        //    ConnectedUserList.TryRemove(Context.ConnectionId, out Value);
        //    Clients.All.getOnlineList(ConnectedUserList);
        //    return base.OnDisconnected(stopCalled);
        //}

        // 使用者離線時觸發此方法
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            UserData value;
            //將使用者從上線清單移除
            ConcurrentedUserList.TryRemove(Context.ConnectionId, out value);

            // 呼叫所有客戶端，更新在線使用者列表
            Clients.All.getOnlineList(ConcurrentedUserList);

            return base.OnDisconnected(stopCalled);
        }

        // 編輯文件時觸發此方法
        public void EditDoc(DocData uploadDocData)
        {
            uploadDocData.lastEditTime = DateTime.Now.ToString("f");
            docData = uploadDocData;

            // 呼叫所有客戶端，記錄更新後的文件內容
            Clients.All.updateDocContent(docData);
        }

        ////當使用者編輯文件呼叫時
        //public void editDoc(DocData uploadDocData)
        //{
        //    uploadDocData.lastEditTime = DateTime.Now.ToString("f");
        //    docData = uploadDocData;
        //    Clients.All.updateDocContent(docData);
        //}
    }
}