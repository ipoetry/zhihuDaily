var gesture;
//记录手势操作开始位置
var gestureStartX;

//触发Id,防止重复触发，触发Id与手势Id
var gestureId = 1;
var lastGestureId = 0;

//速度触发
var gestureVector = 1.5;

//注册手势事件
function prepareTarget(target, eventListener) {
    //var target = document.getElementById(targetId);
    target.addEventListener("MSGestureStart", eventListener, false);
    target.addEventListener("MSGestureEnd", eventListener, false);
    target.addEventListener("MSGestureChange", eventListener, false);
    target.addEventListener("MSInertiaStart", eventListener, false);
    //target.addEventListener("MSGestureTap", eventListener, false);
    //target.addEventListener("MSGestureHold", eventListener, false);
    target.addEventListener("pointerdown", onPointDown, false);
    target.addEventListener("pointerup", onPointUp, false);

    gesture = new MSGesture();
    gesture.target = target;
}

function onPointUp(e) {
    //把触发时间参数传到gesture
    gesture.addPointer(e.pointerId);
}

function onPointDown(e) {
    //把触发时间参数传到gesture
    gesture.addPointer(e.pointerId);
}

//手势事件
//具体的属性参见：https://msdn.microsoft.com/zh-cn/library/ie/hh772076%28v=vs.85%29.aspx
function eventListener(evt) {
    var myGesture = evt.gestureObject;
    if (evt.type == "MSGestureStart") {
        //开始触发，记录初始位置
        gestureStartX = evt.clientX;
    }
    else if (evt.type == "MSInertiaStart") {
        if (lastGestureId == gestureId || evt.velocityX == "undefined") {
            return;
        } else {
            //释放时触发惯性事件，判断手势释放时的速度
            if (evt.velocityX > gestureVector) {
                var jsonObj = { type: "swiperight" };
                window.external.notify(JSON.stringify(jsonObj));
                lastGestureId = gestureId;
            } else if (evt.velocityX < -gestureVector) {
                jsonObj = { type: "swipeleft" };
                window.external.notify(JSON.stringify(jsonObj));
                lastGestureId = gestureId;
            }
        }
    }
    else if (evt.type == "MSGestureChange") {
        //if (lastGestureId == gestureId) {
        //    return;
        //} else {
        //    var change = evt.clientX - gestureStartX;
        //    window.external.notify("clientX:" + change);
        //}
    } else if (evt.type == "MSGestureEnd") {
        //手势结束，Id+1
        gestureId = gestureId + 1;
        myGesture.reset();
    }
}