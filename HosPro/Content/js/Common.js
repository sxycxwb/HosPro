/**
  * ajax封装
  * url 发送请求的地址
  * data 发送到服务器的数据，数组存储，如：{"date": new Date().getTime(), "state": 1}
  * successfn 成功回调函数
  */
jQuery.axs = function (url, data, successfn) {
    data = (data == null || data == "" || typeof (data) == "undefined") ? { "date": new Date().getTime() } : data;
    $.ajax({
        type: "post",
        data: data,
        url: url,
        dataType: "json",
        success: function (d) {
            successfn(d);
        }
    });
};

//显示加载器
function showLoader() {
    //显示加载器.for jQuery Mobile 1.2.0
    $.mobile.loading('show', {
        text: '加载中...', //加载器中显示的文字
        textVisible: true, //是否显示文字
        theme: 'a', //加载器主题样式a-e
        textonly: false, //是否只显示文字
        html: "" //要显示的html内容，如图片等
    });
}

//隐藏加载器.for jQuery Mobile 1.2.0
function hideLoader() {
    //隐藏加载器
    $.mobile.loading('hide');
}