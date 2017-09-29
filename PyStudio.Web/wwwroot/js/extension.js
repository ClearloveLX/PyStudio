/**
JS扩展工具类
*/

/*自定义LayUI组件*/

/*iframe层*/

/**
* 按百分比打开窗口
*/
function percentage(type, centage) {
    var _width = $(window).width();
    var _height = $(window).height();
    if (_width > 992) {
        //PC端返回百分比大小
        if (type == 'width') {
            if (centage <= 100) {
                return _width / 100 * centage;
            } else {
                return centage;
            }
        } else if (type == 'height') {
            if (centage <= 100) {
                return _height / 100 * centage;
            } else {
                return centage;
            }
        }
    } else {
        //平板电脑and手机端返回满屏
        if (type == 'width') {
            return _width;
        } else if (type == 'height') {
            return _height;
        }
    }
}

/**
* 自定义LayUI弹出iframe
*/
function openiframe() {
    //使用arguments判断传过来的参数的Length,重载openiframe。
    if (arguments.length == 2) {
        //标题和路径，默认80%大小
        layer.open({
            type: 2,
            title: arguments[0],
            area: [percentage('width', 80) + 'px', percentage('height', 80) + 'px'],
            fixed: true,
            maxmin: true,
            anim: 1,//0-6的动画形式，-1不开启
            shade: 0.8,
            shadeClose: true,
            content: arguments[1]
        });
        layer.load
    } else if (arguments.length == 4) {
        //标题，自定义大小窗口（如果大于100就写死，小雨100按百分比）
        layer.open({
            type: 2,
            title: arguments[0],
            area: [percentage('width', arguments[1]) + 'px', percentage('height', arguments[2]) + 'px'],
            fixed: true,
            maxmin: true,
            anim: 1,//0-6的动画形式，-1不开启
            shade: 0.8,
            shadeClose: true,
            content: arguments[3]
        });
    }
}

/*自定义提示*/

function openmsg() {
    if (arguments.length == 1) {
        layer.msg(arguments[0]);//只提示文字
    } else if (arguments.length == 2) {
        if (arguments[1] == '0') {
            layer.msg(arguments[0], { icon: 2, time: 1500 });//提示失败
        } else if (arguments[1] == '1') {
            layer.msg(arguments[0], { icon: 1, time: 1000 }, function () { parent.location = parent.location; });//提示成功并且刷新
        } else {
            layer.msg(arguments[0], { icon: 0, time: 1500 });//提示
        }
    }
}


/**
* 判断是否为空 
* @param 判断的值 
*/
function isEmpty(val) {
    return (val === "" || val == undefined || val == null);
}

/**
*获取queryString
*@param 要获取的Key
*@returns 值
*/
function getQueryString(name) {
    var url = encodeURI(location.search); //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest[name];
}