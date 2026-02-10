var UrlParameterPlugin = {
    GetURLParameter: function(namePtr) {
        var name = UTF8ToString(namePtr);
        if (typeof window === 'undefined') return null;

        var urlParams = new URLSearchParams(window.location.search);
        var value = urlParams.get(name);
        
        if (!value) return null;

        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    }
};

mergeInto(LibraryManager.library, UrlParameterPlugin);
