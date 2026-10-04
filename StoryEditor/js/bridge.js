// bridge.js: the only page script that knows which browser it is hosted in. Inlined first into every page.
//  ose.send(type, payload)  page -> C#
//  ose.on(type, handler)    C# -> page; when the message has a 'rid', the handler's return value is the reply
(function () {
    var docId = '__OSE_DOC_ID__';   // replaced by the host on load, so it can ignore an older document's messages
    var handlers = {};

    function post(str) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(str);
        else if (window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.ose)
            window.webkit.messageHandlers.ose.postMessage(str);
        else
            window.external.postMessage(str);
    }

    if (!window.JSON) {
        // IE below document mode 8 has no JSON, so say so the only way we can
        try {
            post('{"type":"jsError","message":"JSON is unavailable (documentMode ' + document.documentMode + ')"}');
        } catch (e) { }
    }

    function send(type, payload) {
        var msg = payload || {};
        msg.type = type;
        msg.doc = docId;
        try {
            post(JSON.stringify(msg));
        } catch (e) { }
    }

    function on(type, handler) {
        handlers[type] = handler;
    }

    window.oseReceive = function (json) {
        var msg, result = null, error = null;
        try {
            msg = JSON.parse(json);
        } catch (e) {
            send('jsError', { message: 'unparseable message from host: ' + e.message });
            return;
        }
        try {
            var handler = handlers[msg.type];
            if (handler)
                result = handler(msg);
            else
                error = 'no handler for ' + msg.type;
        } catch (e) {
            error = msg.type + ': ' + (e.message || String(e));
        }
        if (error)
            send('jsError', { message: error });
        if (msg.rid !== undefined) {
            var reply = result || {};
            reply.re = msg.rid;
            if (error)
                reply.error = error;
            send('reply', reply);
        }
    };

    window.onerror = function (message, source, line) {
        send('jsError', { message: String(message), source: String(source || ''), line: line || 0 });
        return false;   // keep IE's own handling, as before
    };

    function ready() {
        send('ready', { docMode: document.documentMode || 0 });
    }
    if (window.addEventListener)
        window.addEventListener('load', ready, false);
    else
        window.attachEvent('onload', ready);

    window.ose = { send: send, on: on };
})();
