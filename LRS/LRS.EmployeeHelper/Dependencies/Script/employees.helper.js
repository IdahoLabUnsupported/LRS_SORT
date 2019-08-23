
var employeesInit = function (searchUrl, getUrl, id, fieldName) {
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
        // no matter what, we'll need a container to put the select EmployeeIds in
        var $search = $('#' + id + '_search');
        var $par = $search.parent().parent();
        var $msg = $("<em>").attr('id', 'msg-' + fieldName).text("There is no one selected, please use the search below...").prependTo($par).css("display", "block").css("color", "#999");
        var $container = $('<div>').attr('id', 'res-' + fieldName).addClass('chosen-container chosen-container-multi').prependTo($par).hide();
        var $selected = $('<ul>').attr('id', 'ul-' + fieldName).addClass('chosen-choices').appendTo($container).css("display", "block");

        var typeAhead = $('#' + id + '_search').typeahead(null, {
            name: id + '_',
            displayKey: 'NameFull',
            valueKey: 'EmployeeId',
            source: engine,
            limit: 10
        }).bind('typeahead:select', function (ev, suggestion) {
            //check to see if this is a duplicate
            var values = $('[name="' + fieldName + '"]');
            var duplicate = false;
            $.each(values, function (index, element) {
                if (this.value == suggestion.EmployeeId) {
                    duplicate = true;
                    return false;
                }
            });
            if (!duplicate) {
                addEmployeeId(suggestion);
            }
            $('#' + id).val('');
            typeAhead.typeahead('val', '');
        }).bind('typeahead:autocompleted', function (ev, suggestion) {
            $('#' + id).val(suggestion.EmployeeId).trigger('change');
        }).bind('typeahead:change', function () {
            if ($('#' + id).val() == '') {
                $(this).val('');
            }
        }).keyup(function (event) {
            if ($('#' + id).val() != '' && !(event.which == 9 || event.which == 13 || event.which == 16 || event.which == 17
                || event.which == 18 || event.which == 19 || event.which == 20 || event.which == 27 || event.which == 33 || event.which == 34
                || event.which == 35 || event.which == 36 || event.which == 37 || event.which == 38 || event.which == 39 || event.which == 40
                || event.which == 45 || event.which == 144 || event.which == 145)) {
                $('#' + id).val('').trigger('change');
            }
        }).blur(function (event) {
            if ($('#' + id).val() == '') {
                $(this).val('');
            } else {
                engine.search(
                    $('#' + id).val(),
                    function (res) { },
                    function (res) {
                        if (res.length == 1) {
                            $('#' + id + '_search').val(res[0].NameFull);
                            $('#' + id).val(res[0].EmployeeId);
                        }
                        else {
                            $('#' + id).val('');
                        }
                    }
                );
            }
        });

        var addEmployeeId = function (s) {
            $msg.hide();
            $x = $('<li>').text(s.NameFull).addClass('search-choice').appendTo($selected);
            $('<input>').attr('name', fieldName).attr('type', 'hidden').val(s.EmployeeId).appendTo($x);
            $('<a>').addClass('search-choice-close')
                .click(function () { removeEmployeeId(this) }).appendTo($x);
            $container.show();
        }

        var removeEmployeeId = function (s) {
            $(s).closest('li').remove();
            typeAhead.typeahead('val', '');
            $search.focus();
            //check to see if there are any selections and to close the multiselect
            var values = $('[name="' + fieldName + '"]');
            if (values.length == 0) {
                $msg.show();
                $container.hide('fast');
            }
            return false;
        }

        if ($search.val() != '') {
            $.post(getUrl, { EmployeeIds: $search.val() }, function (res) {
                for (var i = 0; i < res.length; i++) {
                    addEmployeeId(res[i]);
                }
            });
            $search.val('');
            typeAhead.typeahead('val', '');
        }
    });
}

