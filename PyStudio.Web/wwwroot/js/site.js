// Write your JavaScript code.
var pyStudio = function () {
    var now = new Date();
    return {
        bindSubmitBtn: function () {
            $("button[id='Save']").on('click', function () {
                var _btn = $(this);
                var _msg = $("#msgBox");
                _btn.find('span').text("提交中，请稍后...");
                var _form = $("form[name='form_submit']");
                if (_form.valid()) {
                    _form.submit();
                } else {
                    _btn.find('span').text("提 交");
                }
            });
        },
        getUserInfo: function () {
            $.post("/api/AccountApi/GetLoginInfo", function (data) {
                var _html = '';
                if (data.data != null && data.isOK == 1) {
                    var _userInfo = data.data;
                    _html = '<img src="' + _userInfo.userHeadPhoto +'" alt="user-image" class="img-circle img-inline userpic-32" width="28" />\
                                <span>\
                                    '+ _userInfo.userName +'\
                                    <i class="fa-angle-down"></i>\
                                </span>';
                }
                
                if (!isEmpty(_html)) {
                    $('a#userinfo').html(_html);
                }
            });
        }
    }
}

var lxPyStudio = new pyStudio();
//提交按钮
lxPyStudio.bindSubmitBtn();
//获取用户信息
lxPyStudio.getUserInfo();