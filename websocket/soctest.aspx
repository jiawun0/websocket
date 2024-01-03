<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="soctest.aspx.cs" Inherits="websocket.soctest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>SignalR Sample</title>
    <script src="Scripts/jquery-1.6.4.js"></script>
    <script src="Scripts/jquery.signalR-2.4.3.js"></script>
    <script src='/signalr/js'></script>
</head>

<body>
    <form id="form1" runat="server">
        <div>
            <%--上線人員清單、上次編輯時間、編輯文件區域--%>
            <ul class="online-user-list"></ul>
            <span class="last-edit-time"></span>
            <span class="last-edit-user"></span>
        </div>
        <div class="edit-area">
            <!-- 單純的標籤(label)來顯示內容 -->
            <label class="edit-doc-label"></label>

            <!-- 連動的文字輸入框 -->
            <input type="text" id="linkedTextbox" />
            <input type="button" id="sendButton" value="Send" />
        </div>

        <script>
            //var userName = "";
            //$(function () {
            //    while (userName.length == 0) {
            //        userName = window.prompt("請輸入使用者名稱");
            //        if (!userName) userName = "";
            //    }
            //});
            var userName = ""; // 儲存使用者名稱的變數
            $(function () {
                // 在頁面載入時請求使用者輸入名稱
                userName = window.prompt("請輸入使用者名稱");
                if (!userName) userName = ""; // 若未輸入名稱，將其設為空字串
            });

            //建立與server端的hub物件，注意hub的關鍵字母一定要小寫
            var syscomDocHub = $.connection.syscomHub1;

            //將連線打開
            $.connection.hub.start()
                .done(function () {
                    //當連線完成，呼叫server端的userConnected方法，並傳送使用者姓名透過JSON Object傳到Server端
                    syscomDocHub.server.onJoinRoom({ Name: userName });
                })
                .fail(function () {
                    alert("Error connecting to realtime service!");
                });

            //取得所有上線清單，將上線清單的Dictionary物件傳至前端，透過遍歷物件的方式，將使用者清單列在畫面上
            syscomDocHub.client.getOnlineList = function (userList) {
                $('.online-user-list').html('');
                $.each(userList, function (index, data) {
                    $('.online-user-list').append('<li id = "' + data.Sid + '">' + data.Name + '</li>');
                });
            }

            //更新文件內容
            syscomDocHub.client.updateDocContent = function (docObject) {
                $(".last-edit-time").html(docObject.lastEditTime);
                $(".last-edit-user").html(docObject.lastEditName);
                // 使用 label 顯示內容
                $(".edit-doc-label").text(docObject.content);
            }

            // 監聽按鈕點擊事件
            $("#sendButton").click(function () {
                var content = $("#linkedTextbox").val();

                // 同時更新 label 顯示內容
                $(".edit-doc-label").text(content);

                syscomDocHub.server.editDoc({
                    lastEditId: $.connection.hub.id,
                    lastEditName: userName,
                    content: content
                });
            });
        </script>

    </form>
</body>
</html>