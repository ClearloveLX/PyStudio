// Write your JavaScript code.
var pyStudio = function () {
    var now = new Date();
    return {
        bindSubmitBtn: function () {
            $("button[id='Save']").on('click', function () {
                var _btn = $(this);
                console.log(_btn.find('span').text());
                var _msg = $("#msgBox");
                _btn.find('span').text("提交中，请稍后...");
                var _form = $("form[name='form_submit']");
                console.log(_form);
                if (_form.valid()) {
                    _form.submit();
                } else {
                    _btn.find('span').text("提 交");
                }
            });
        }
    }
}

var lxPyStudio = new pyStudio();
//提交按钮
lxPyStudio.bindSubmitBtn();