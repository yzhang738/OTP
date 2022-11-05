    function SetRingCookie(key, value, options) {

        // key and value given, set cookie...
        if (arguments.length > 1 && (value === null || typeof value !== "object")) {
            options = jQuery.extend({}, options);

            if (value === null) {
                options.expires = -1;
            }

            if (typeof options.expires === 'number') {
                var days = options.expires, t = options.expires = new Date();
                t.setDate(t.getDate() + days);
            }

            return (document.cookie = [
                encodeURIComponent(key), '=',
                options.raw ? String(value) : encodeURIComponent(String(value)),
                options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
                options.path ? '; path=' + options.path : '',
                options.domain ? '; domain=' + options.domain : '',
                options.secure ? '; secure' : ''
                ].join(''));
        }

        // key and possibly options given, get cookie...
        options = value || {};
        var result, decode = options.raw ? function (s) { return s; } : decodeURIComponent;
        return (result = new RegExp('(?:^|; )' + encodeURIComponent(key) + '=([^;]*)').exec(document.cookie)) ? decode(result[1]) : null;
    };

    function changeLang(lang) {
        SetRingCookie('ck_lang', lang, { path: '/' });
        window.location.reload();   
        return false;
    };

    function changeSport(sport, path) {
        SetRingCookie('ck_sport', sport, { path: '/' });
        window.location = path;
        return false;
    };

    function changeYear() {
        var editor = $(this).data('tDropDownList');
        SetRingCookie('ck_year', editor.text(), { path: '/' });

		//removes the id field from URL which was causing the wrong data to be displayed
        var URL_array = new Array();
		var URL = window.location.toString();
		URL_array = URL.split('/');
		if (URL_array.length == 6) {
			if (URL_array[3] == "Evaluations" && URL_array[4] == "Summary") {
				window.location.href = '/Evaluations/Summary/';
				return false;
			}
		}
		
        window.location.reload();
        return false;
    };

    function changeActionItemStatus() {
        var editor = $(this).data('tDropDownList');
        SetRingCookie('ck_ais', editor.value(), { path: '/' });
        window.location.reload();
        return false;
    };

    function grid_OnError(e) {
        OTPAlert(parseOnErrorMessage(e.XMLHttpRequest.responseText), "An Error Has Occured");
        e.preventDefault();
        return;
    }

    function parseOnErrorMessage(message) {
        var startIndex = message.indexOf("<title>") + 7;
        var endIndex = message.indexOf("</title>");
        return message.substring(startIndex, endIndex);
    }

    String.prototype.format = function ()
    {
        var s = this,
        i = arguments.length;

        while (i--)
        {
            s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
        }
        return s;
    };

    String.prototype.isNumber = function ()
    {
        var intRegex = /^\d+$/;
        var floatRegex = /^((\d+(\.\d *)?)|((\d*\.)?\d+))$/;

        if (intRegex.test(this) || floatRegex.test(this))
        {
            return true;
        }
        else
        {
            return false;
        }
    };

