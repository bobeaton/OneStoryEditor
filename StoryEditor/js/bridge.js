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

    // For IE quirks mode only (documentMode 5, which the pane pages have always run in): it has no JSON object.
    //  Only our own messages go through this: plain objects, arrays, strings, numbers, booleans and null, and
    //  what we parse always comes from our own C# (Newtonsoft output).
    var json = window.JSON || (function () {
        function quote(s) {
            return '"' + s.replace(/[\\"\x00-\x1f]/g, function (c) {
                if (c == '\\' || c == '"')
                    return '\\' + c;
                return '\\u' + ('000' + c.charCodeAt(0).toString(16)).slice(-4);
            }) + '"';
        }
        function stringify(v) {
            if (v === null)
                return 'null';
            switch (typeof v) {
                case 'string': return quote(v);
                case 'number': return isFinite(v) ? String(v) : 'null';
                case 'boolean': return String(v);
                case 'object':
                    var a = [], i;
                    if (Object.prototype.toString.call(v) == '[object Array]') {
                        for (i = 0; i < v.length; i++)
                            a.push((v[i] === undefined || typeof v[i] == 'function') ? 'null' : stringify(v[i]));
                        return '[' + a.join(',') + ']';
                    }
                    for (i in v) {
                        if (Object.prototype.hasOwnProperty.call(v, i) && v[i] !== undefined && typeof v[i] != 'function')
                            a.push(quote(i) + ':' + stringify(v[i]));
                    }
                    return '{' + a.join(',') + '}';
            }
            return undefined;
        }
        // U+2028 and U+2029 are valid in JSON strings but end a line in script source
        var lineSeparators = new RegExp('[' + String.fromCharCode(0x2028, 0x2029) + ']', 'g');
        function parse(text) {
            text = String(text).replace(lineSeparators, function (c) { return '\\u' + c.charCodeAt(0).toString(16); });
            // json2's check that the text is only JSON before evaluating it
            if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                                         .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                                         .replace(/(?:^|:|,)(?:\s*\[)+/g, '')))
                return new Function('return ' + text)();
            throw new SyntaxError('not JSON');
        }
        return { stringify: stringify, parse: parse };
    })();

    function send(type, payload) {
        var msg = payload || {};
        msg.type = type;
        msg.doc = docId;
        try {
            post(json.stringify(msg));
        } catch (e) { }
    }

    function on(type, handler) {
        handlers[type] = handler;
    }

    window.oseReceive = function (str) {
        var msg, result = null, error = null;
        try {
            msg = json.parse(str);
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

    // the nearest element (from el up) for which test(el) is true, or null
    function closest(el, test) {
        while (el && el.nodeType == 1) {
            if (test(el))
                return el;
            el = el.parentNode;
        }
        return null;
    }

    // target.addEventListener where there is one; otherwise (IE quirks mode) attachEvent, with the handler given an
    //  event object that has the same few properties the pages read (target, button, ctrlKey, keyCode). Under
    //  attachEvent a capture-phase 'focus' becomes 'focusin' (which bubbles)
    function listen(target, type, handler, useCapture) {
        if (target.addEventListener) {
            target.addEventListener(type, handler, !!useCapture);
            return;
        }
        if (useCapture && (type == 'focus'))
            type = 'focusin';
        target.attachEvent('on' + type, function (ev) {
            ev = ev || window.event;
            handler({
                type: ev.type, target: ev.srcElement || (window.event && window.event.srcElement) || null,
                button: ev.button, ctrlKey: ev.ctrlKey, keyCode: ev.keyCode, originalEvent: ev
            });
        });
    }

    function cancel(e) {
        if (e.preventDefault)
            e.preventDefault();
        e.returnValue = false;
        if (e.originalEvent)
            e.originalEvent.returnValue = false;
        try {
            if (window.event)
                window.event.returnValue = false;
        } catch (ex) { }
    }

    on('scrollTo', function (m) {
        var el = document.getElementById(m.id);
        if (!el)
            return { found: false };
        // later, so it runs after anything else waiting to scroll the document (what Application.DoEvents was for)
        setTimeout(function () {
            el.scrollIntoView(m.alignTop !== false);
            if (m.focus) {
                try { el.focus(); } catch (e) { }
            }
        }, 0);
        return { found: true };
    });

    function ready() {
        send('ready', { docMode: document.documentMode || 0 });
    }
    listen(window, 'load', ready, false);

    window.ose = { send: send, on: on, closest: closest, cancel: cancel, listen: listen };
})();
