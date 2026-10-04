// modern/bridge.js: the WebView2 (Chromium) bridge -- the only modern script that names its host. Inlined first into
//  every page when the host is WebView2. Same API as js/bridge.js (the IE one):
//  ose.send(type, payload)  page -> C#
//  ose.on(type, handler)    C# -> page; when the message has a 'rid', the handler's return value is the reply
(function () {
    'use strict';
    var docId = '__OSE_DOC_ID__';   // replaced by the host on load, so it can ignore an older document's messages
    var handlers = {};

    function send(type, payload) {
        var msg = {}, k;
        payload = payload || {};
        for (k in payload)
            if (Object.prototype.hasOwnProperty.call(payload, k))
                msg[k] = payload[k];
        msg.type = type;
        msg.doc = docId;
        try {
            window.chrome.webview.postMessage(JSON.stringify(msg));
        } catch (e) { }
    }

    function on(type, handler) {
        handlers[type] = handler;
    }

    function receive(str) {
        var msg, result = null, error = null;
        try {
            msg = JSON.parse(str);
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
            var reply = (result && typeof result === 'object') ? result : {};
            reply.re = msg.rid;
            if (error)
                reply.error = error;
            send('reply', reply);
        }
    }

    window.chrome.webview.addEventListener('message', function (e) { receive(e.data); });

    window.addEventListener('error', function (e) {
        send('jsError', { message: String(e.message), source: String(e.filename || ''), line: e.lineno || 0 });
    });

    // the nearest element (from el up) for which test(el) is true, or null
    function closest(el, test) {
        while (el && el.nodeType === 1) {
            if (test(el))
                return el;
            el = el.parentNode;
        }
        return null;
    }

    function cancel(e) {
        e.preventDefault();
    }

    function listen(target, type, handler, useCapture) {
        target.addEventListener(type, handler, !!useCapture);
    }

    on('scrollTo', function (m) {
        var el = document.getElementById(m.id);
        if (!el)
            return { found: false };
        setTimeout(function () {
            el.scrollIntoView(m.alignTop !== false);
            if (m.focus) {
                try { el.focus(); } catch (e) { }
            }
        }, 0);
        return { found: true };
    });

    window.addEventListener('load', function () {
        send('ready', { docMode: 0 });
    });

    window.ose = { send: send, on: on, closest: closest, cancel: cancel, listen: listen };
})();
