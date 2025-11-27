window.ScrollToBottom = (elementName) => {
    element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
}


function scrollAsync() {
    var isSyncingLeftScroll = false;
    var isSyncingRightScroll = false;
    var leftDiv = document.getElementById('left');
    var rightDiv = document.getElementById('right');

    leftDiv.onscroll = function () {
        if (!isSyncingLeftScroll) {
            isSyncingRightScroll = true;
            rightDiv.scrollTop = this.scrollTop;
        }
        isSyncingLeftScroll = false;
    }

    rightDiv.onscroll = function () {
        if (!isSyncingRightScroll) {
            isSyncingLeftScroll = true;
            leftDiv.scrollTop = this.scrollTop;
        }
        isSyncingRightScroll = false;
    }
};