
var employeeInit = function (searchUrl, fieldName) {
    var engine = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('EmployeeId'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: searchUrl + '?text=%QUERY',
            wildcard: '%QUERY',
            transport: function (opts, onSuccess, onError) {
                var url = opts.url.split('?')[0];
                var query = opts.url.split("=")[1];
                $.ajax({
                    url: url,
                    data: 'text=' + query,
                    type: 'POST',
                    success: onSuccess,
                    error: onError
                });
            }
        }
    });
    $(document).ready(function () {
        $('#' + fieldName + '_search').typeahead(null, {
            name: fieldName + '_',
            displayKey: 'NameFull',
            valueKey: 'EmployeeId',
            source: engine,
            limit: 10
        }).bind('typeahead:select', function (ev, suggestion) {
            $('#' + fieldName).val(suggestion.EmployeeId).trigger('change');
        }).bind('typeahead:autocompleted', function (ev, suggestion) {
            $('#' + fieldName).val(suggestion.EmployeeId).trigger('change');
        }).bind('typeahead:change', function () {
            if ($('#' + fieldName).val() == '') {
                $(this).val('');
            }
        }).keyup(function (event) {
            if ($('#' + fieldName).val() != '' && !(event.which == 9 || event.which == 13 || event.which == 16 || event.which == 17
                || event.which == 18 || event.which == 19 || event.which == 20 || event.which == 27 || event.which == 33 || event.which == 34
                || event.which == 35 || event.which == 36 || event.which == 37 || event.which == 38 || event.which == 39 || event.which == 40
                || event.which == 45 || event.which == 144 || event.which == 145)) {
                $('#' + fieldName).val('').trigger('change');
            }
        }).blur(function (event) {
            if ($('#' + fieldName).val() == '') {
                $(this).val('');
            } else {
                engine.search(
                    $('#' + fieldName).val(),
                    function (res) { },
                    function (res) {
                        if (res.length == 1) {
                            $('#' + fieldName + '_search').val(res[0].NameFull);
                            $('#' + fieldName).val(res[0].EmployeeId);
                        }
                        else {
                            $('#' + fieldName).val('');
                        }
                    }
                );
            }
        });
        if ($('#' + fieldName).val() != '') {
            // while we are initializing, if there is a value, we'll go get the display
            engine.search(
                $('#' + fieldName).val(),
                function (res) { },
                function (res) {
                    if (res.length == 1) {
                        $('#' + fieldName + '_search').val(res[0].NameFull);
                        $('#' + fieldName).val(res[0].EmployeeId);
                    }
                    else {
                        $('#' + fieldName).val('');
                    }
                }
            );
        }
    });
}