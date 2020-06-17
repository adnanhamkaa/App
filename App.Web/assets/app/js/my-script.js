window.InitDateTimePicker = function (elem, style, watch, opt, onchange, cb) {

    if (isNotUndefined(onchange) || typeof (onchange) !== 'function') onchange = function () { };

    if (!isNotUndefined(opt)) {
        opt = {};
    }

    var inp = style === 'time'
        ? $(elem).find('input[type=hidden].clock-input')
        : $(elem).find('input[type=hidden].datetime-input');
    var inpDisp = style === 'time'
        ? $(elem).find('input[type=text].clock-display')
        : $(elem).find('input[type=text].datetime-display');
    var tog = style === 'time' ? $(elem).find('.clock-toggle') : $(elem).find('.datetime-toggle');
    var res = style === 'time' ? $(elem).find('.clock-reset') : $(elem).find('.datetime-reset');
    var mindate = style === 'date' ? $(inpDisp).data('mindate') : null;


    window[elem] = null;

    if (style === 'time') {
        $(inpDisp).clockpicker({
            donetext: 'Pilih',
            autoclose: true,
            afterDone: function () {
                var items = _.split($(inpDisp).val(), ':');
                var hh = items[0] * 1;
                var mm = items[1] * 1;
                var locmom = moment([
                    2000, 5, 15, hh, mm, 0
                ]);
                $(inp).val(locmom.toISOString());
            }
        });
        $(tog).click(function (ev) {
            ev.stopPropagation();
            $(inpDisp).clockpicker('show');
        });
    } else {
        var your_dates = [new Date(2020, 7, 7)];
        var dtopts = {
            //isRTL: true,
            format: 'd M yyyy, hh:ii',
            autoclose: true,
            todayBtn: 'linked',
            fontAwesome: true,
            pickerPosition: 'bottom-left',
            orientation: "bottom",
            minuteStep: 5,
            language: 'id',
            inline: isNotUndefined(opt.inline) ? opt.inline : false,
            multidate: opt.useMultipleDate,
            todayHighlight: false,
            beforeShowDay: function (date) {
                if ($.inArray(date, your_dates)) {
                    return [true, 'css-class-to-highlight', 'tooltip text'];
                } else {
                    return [true, '', ''];
                }
            }
        };

        var inlineformat = 'DD MMM YYYY, HH:mm';
        if (style === 'date') {
            dtopts.minView = 2;
            dtopts.format = 'd M yyyy';
            inlineformat = 'DD MMM YYYY';
        }

        if (style === 'month') {
            dtopts.minView = undefined;
            dtopts.minViewMode = 1;
            dtopts.startView = 1;
            dtopts.format = 'MM yyyy';
            inlineformat = 'MM yyyy';
            dtopts.todayBtn = false;
        }

        if (style === 'year') {
            dtopts.minView = undefined;
            dtopts.minViewMode = 2;
            dtopts.startView = 2;
            dtopts.format = 'yyyy';
            inlineformat = 'yyyy';
            dtopts.todayBtn = false;
        }

        if (mindate) {
            dtopts.minDate = moment(mindate, 'YYYY-MM-DD').toDate();
            dtopts.startDate = moment(mindate, 'YYYY-MM-DD').toDate();
        }

        if (opt.useworkdays) {
            dtopts.daysOfWeekDisabled = [0, 6]
            var system = $("#SystemSettingWorkDay :selected").val();
            var year = $("#YearSettingWorkDay :selected").val();
            var id = $("#IdSettinngWorkday").val();
            var url = '/general/GetHolidays?module=' + opt.module + '&year=' + year;
            if (opt.useMultipleDate) {
                url = '/general/GetSelectedDate?id=' + id + '&system=' + system + '&year=' + year;
            }
            $.ajax({
                url: url,
                method: 'POST',
                success: function (data) {
                    if (Array.isArray(data)) {
                        dtopts.datesDisabled = data.map(function (obj) {
                            return moment.utc(obj.HolidayDate).toDate();//.add('days', 1).toDate();
                        });
                        try {
                            window.holidayDates = data.map(function (obj) {
                                return moment.utc(obj.HolidayDate);
                            });
                        } catch (ex) {

                        }

                    }

                    var temporaryDateNowSelected;

                    var idcontainer = dtopts.inline ? opt.inlinecontainer ? opt.inlinecontainer : ('#' + opt.id + '-inlinebox') : inpDisp

                    if (style == 'date' && opt.useMultipleDate) {
                        instant = $(idcontainer).datepicker(dtopts);
                        $(idcontainer).attr('data-datepicker', 'init');
                        var selectedDate = document.getElementById("selectedDateString").value;
                        var selectedDateList = selectedDate.split(", ");

                        var listSelectedDate = [];
                        selectedDateList.forEach(function (date) {
                            listSelectedDate.push(moment(date, "MM/DD/YYYY hh:mm:ss A").toDate());
                        });
                        data.forEach(function (date) {
                            if (listSelectedDate.includes(moment(date).toDate()) == false) {
                                listSelectedDate.push(moment.utc(date).toDate());
                            }
                        });
                        
                        listSelectedDate.forEach(function (date) {
                            if (date instanceof Date && !isNaN(date.valueOf())) {
                                if (temporaryDateNowSelected) {
                                    if (temporaryDateNowSelected <= date) {
                                        temporaryDateNowSelected = date;
                                    }
                                } else {
                                    temporaryDateNowSelected = date;
                                }
                            }
                        });

                        if (!temporaryDateNowSelected) {
                            console.log(temporaryDateNowSelected);
                            temporaryDateNowSelected = new Date();
                        }

                        $(idcontainer).datepicker('setDatesDisabled', listSelectedDate);
                        //$(idcontainer).datepicker('todayHighlight', false);
                        console.log("Sipoke");
                        $(idcontainer).datepicker('setDate', temporaryDateNowSelected);
                    } else if (style == 'month') {
                        window[elem] = $(idcontainer).datepicker(dtopts);
                        $(idcontainer).attr('data-datepicker', 'init');
                        $(idcontainer).datepicker('setDate', moment($(inp).val(), "YYYY-MM-DDTHH:mm:ss").toDate());
                    } else if (style == 'date') {
                        window[elem] = $(idcontainer).datepicker(dtopts);
                        $(idcontainer).attr('data-datepicker', 'init');
                        $(idcontainer).datepicker('setDate', moment($(inp).val(), "YYYY-MM-DDTHH:mm:ss").toDate());
                    } else {
                        window[elem] = $(idcontainer).datetimepicker(dtopts);
                        $(idcontainer).attr('data-datepicker', 'init');
                        $(idcontainer).datetimepicker('setDate', moment($(inp).val(), "YYYY-MM-DDTHH:mm:ss").toDate());
                    }

                    if (opt.useMultipleDate) {
                        console.log("masuk");
                        instant.on('changeDate',
                            function (ev) {
                                var dateMultiple = '';
                                if (ev.dates) {
                                    var counter = 0;
                                    console.log("lllaa" + temporaryDateNowSelected.getTime());
                                    ev.dates.forEach(function (date) {
                                        console.log("lllee" + date.getTime());
                                        if (temporaryDateNowSelected.getTime() === date.getTime()) {
                                            console.log("shit");
                                            return;
                                        }
                                        if (counter > 0) {
                                            dateMultiple = dateMultiple + ', ' + moment(date.toISOString()).format(inlineformat);
                                        } else {
                                            dateMultiple = moment(date.toISOString()).format(inlineformat);
                                        }
                                        counter++;
                                    });
                                    $(inp).val(ev.date.toISOString());
                                } else {
                                    $(inp).val('');
                                }

                                if (dtopts.inline) {
                                    console.log(dateMultiple);
                                    $('#temporaryDateUpdate').val(dateMultiple);
                                    $(inpDisp).val(dateMultiple);
                                }

                                inp.trigger('change');
                                $(inpDisp).trigger('dp.change');

                                if (watchChanged) $(inp).attr('data-changed', 'changed');

                            });
                    } else {
                        window[elem].on('changeDate',
                            function (ev) {
                                if (ev.date) {
                                    $(inp).val(ev.date.toISOString());
                                } else {
                                    $(inp).val('');
                                }

                                if (dtopts.inline) {
                                    $(inpDisp).val(moment(ev.date.toISOString()).format(inlineformat));
                                }

                                //console.log(ev.date);

                                inp.trigger('change');
                                $(inpDisp).trigger('dp.change');

                                if (watchChanged) $(inp).attr('data-changed', 'changed');

                            });
                    }

                    if (typeof (cb) === 'function') cb(window[elem]);

                    try {
                        opt.initcb();
                    } catch (e) {

                    }

                    if (typeof (opt.initcb) === 'function') opt.initcb();
                    $(tog).click(function (ev) {
                        //ev.stopPropagation();
                        if (style == 'month' || style == 'date')
                            $(inpDisp).datepicker('show');
                        else
                            $(inpDisp).datepicker('show');
                    });
                }
            });
        } else {

            var idcontainer = dtopts.inline ? opt.inlinecontainer ? opt.inlinecontainer : ('#' + opt.id + '-inlinebox') : inpDisp
            if (style == 'month' || style == 'year' || style == 'date') {
                window[elem] = $(idcontainer).datepicker(dtopts);
            } else {
                window[elem] = $(idcontainer).datetimepicker(dtopts);
            }
            $(idcontainer).datepicker('setDate', moment($(inp).val(), "YYYY-MM-DDTHH:mm:ss").toDate());
            window[elem].on('changeDate',
                function (ev) {
                    if (ev.date) {
                        $(inp).val(ev.date.toISOString());
                    } else {
                        $(inp).val('');
                    }

                    if (dtopts.inline) {
                        $(inpDisp).val(moment(ev.date.toISOString()).format(inlineformat));
                    }

                    onchange();
                    //console.log(ev.date);

                    $(inp).trigger('change');
                    $(inpDisp).trigger('dp.change');

                    if (watchChanged) $(inp).attr('data-changed', 'changed');

                });

            
            $(tog).click(function (ev) {
                //ev.stopPropagation();
                if (style == 'month' || style == 'year' || style == 'date')
                    $(inpDisp).datepicker('show');
                else
                    $(inpDisp).datetimepicker('show');
            });
        }

    }
    if (res) {
        $(res).click(function (ev) {
            ev.stopPropagation();
            $(inpDisp).val('');
            $(inp).val('');
            $(inp).trigger('change');
        });
    }

    //return instant;
}

function isChanged(elm) {
    return elm.data('changed') === 'changed';
}

function confirmDialog(msg) {
    if (!confirm(msg)) {
        return false;
    }
}

function insertParam(key, value, location) {
    key = encodeURI(key); value = encodeURI(value);

    var kvp = location.search.substr(1).split('&');

    var i = kvp.length; var x; while (i--) {
        x = kvp[i].split('=');

        if (x[0] == key) {
            x[1] = value;
            kvp[i] = x.join('=');
            break;
        }
    }

    if (i < 0) { kvp[kvp.length] = [key, value].join('='); }

    //this will reload the page, it's likely better to store this until finished
    return kvp.join('&');
}

function TextListenTo(source, target) {

    $(source).change(function () {
        $(target).val($(this).val());
    })

}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

function RadioListenTo(source, target) {

    $(source).change(function () {
        var source = this;
        $(target).each(function () {
            if ($(this).val() === $(source).val()) {
                $(this).prop('checked', true);
            }
            else {
                $(this).prop('checked', false);
            }

        });

        $(target + ':not(:checked)').attr('disabled', true);
        $(target + ':checked').attr('disabled', false);

    })

}

function ToJavaScriptDate(value) {
    if (typeof (value) !== 'undefined') {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return (dt.getDate() + "/" + dt.getMonth() + 1) + "/" + dt.getFullYear();
    }
    return null;
}

function isNotUndefined(value) {

    return typeof (value) !== 'undefined';

}

window.flashMessage = function (target, options) {
    //var target = this;
    //debugger;
    options = $.extend({}, options, { timeout: 7000, alert: 'info' });



    if (!options.message) {
        setFlashMessageFromCookie(options);
    }

    if (options.message) {
        if (options.alert === 'File')
            $(target).addClass("alert-success");
        else
            $(target).addClass("alert-" + options.alert.toString().toLowerCase());

        var title = '';

        switch (options.alert) {
            case 'Danger':
                title = 'Error';
                break;
            case 'Warning':
                title = 'Peringatan';
                break;
            case 'Success':
                title = 'Berhasil';
                break;
            case 'Info':
                title = 'Info';
                break;
            case 'File':
                //title = 'File';
                options.timeout = 0;
            case 'Reminder':
                //title = 'File';
                options.timeout = 0;
            default:
                break;
        }

        $('strong', target).html(title);
        if (typeof options.message === "string") {
            target.append(options.message);
        } else {
            target.empty().append(options.message);
        }
    } else {
        return;
    }

    if (options.alert === 'Reminder') {
        loadReminder();
        return;
    }

    if (target.children().length === 0) return;

     else if (options.alert !== 'File')
        target.fadeIn().one("click", function () {
            $(this).fadeOut();
        });
    else
        target.fadeIn();
    if (options.timeout > 0) {
        //setTimeout(function () { target.fadeOut(); }, options.timeout);
    }

    return target;
}

function loadReminder() {

    $('#reminderbody').html('')

    $.ajax({
        url: '/Notification/GetTodos',
        type: 'POST',
        success: function (result) {
            var newnotif = result.length > 0 && $.grep(result, function (obj) { return obj.Status == 'created' }).length;
            debugger;

            result.sort((a, b) => (a.last_nom > b.last_nom) ? 1 : ((b.last_nom > a.last_nom) ? -1 : 0));

            if (result.length > 0) {

                $.each(result, function (i, v) {
                    var template = null;

                    var parent = $('.notifparent[data-type="' + v.DisplayGrouping + '"]');
                    debugger;
                    if (parent.length <= 0) {
                        parent = $('#parenttemplate').clone();
                        parent.attr('id', null);
                        parent.attr('data-type', v.DisplayGrouping);
                        parent.appendTo('#reminderbody');
                        if (v.DisplayGrouping)
                            parent.find('.notiftitle').html(v.DisplayGrouping.replace('Reminder_', '').replaceAll('_', ' '));
                        else
                            parent.find('.notiftitle').html('-');
                    }

                    var parentBody = parent.find('.notifbody');

                    if (v.Status === 'created') {
                        template = $('#remindercreatedtemplate').clone();
                        template.attr('id', null);
                        template.find('.reminderlink').attr('href', v.Url);
                        template.find('.reminderlink').html(v.Keterangan);
                        //template.find('.title').html(v.Keterangan + '<br/><i><i class="la la-clock-o" style="font-size:small;"></i> ' + moment(v.DueDate, "YYYY-MM-DDTHH:mm:ss").format("dddd, DD MMM YYYY") + "</i>");
                        //template.find('.setdone').attr('data-id', v.Id);

                        template.appendTo(parentBody);

                    } else {
                        template = $('#reminderdonetemplate').clone();
                        template.attr('id', null);
                        template.find('.reminderlink').attr('href', v.Url);
                        template.find('.reminderlink').html(v.Keterangan);

                        template.appendTo(parentBody);

                    }
                })
            } else {

                $('#reminderbody').attr('class', '');
                $('#reminderbody').css('text-align', 'center');
                //style = "text-align:center;"
                //template = $('#emptytemplate').clone();
                $('#reminderbody').html($('#emptytemplate').html());
            }

            $('#reminderModal').modal('show');
        }
    })
}



// Get the first alert message read from the cookie
function setFlashMessageFromCookie(options) {
    $.each(new Array('Success', 'Danger', 'Warning', 'Info', 'File','Reminder'), function (i, alert) {
        var cookie = $.cookie("Flash." + alert);

        if (cookie) {
            options.message = cookie;
            options.alert = alert;

            deleteFlashMessageCookie(alert);
            return;
        }
    });
}

// Delete the named flash cookie
function deleteFlashMessageCookie(alert) {
    $.cookie("Flash." + alert, null, { path: '/' });
}

$(document).ready(function () {
    //tandai tab merah bila ada input yang error
    $('.tab-pane').each(function () {
        if ($(this).find('.form-control-feedback').length > 0) {
            $('a.nav-link[href="#' + $(this).attr('id') + '"]').addClass('tabfeedback');
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if ($.fn.dataTable) $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    });

})

function showPreview(sections) {
    var preview = $('#previewwindow');
    preview.html('');

    $.grep(sections, function (v, i) {

        var v2 = $(v).clone();

        $(v2).find('select').each(function (i, select) {
            $(select).find('option').each(function (i, opt) {

                ////debugger;
                var val = $(select).val();
                ////debugger;
                if (val === $(opt).attr('value'))
                    $(opt).attr('selected', 'selected');
                else
                    $(opt).removeAttr('selected');
            });

        });


        var title = $(v2).data('title') || '';

        if (!title) {
            var tabs = $(v2).parents('.tab-pane');
            if (tabs.length > 0) {
                title = $('a.nav-link[href="#' + tabs[0].id + '"]').html();
            }
        }

        $(v2).find('select').each(function (i, select) {
            $(select).find('option').each(function (i, opt) {

                ////debugger;
                var val = $(select).val();
                ////debugger;
                if (val === $(opt).attr('value'))
                    $(opt).attr('selected', 'selected');
                else
                    $(opt).removeAttr('selected');
            });

        });

        preview.append('<div class="row"><div class="col-md-12"><h3 class="preview-title">' + title + '</h3></div></div>');

        v2.removeClass().appendTo('#previewwindow');

    })

    $("#previewwindow :input").attr("disabled", true);
    $("#previewwindow :input").attr("disabled", true);

    $("#previewwindow .btn").remove();

    $('#modal_preview').modal('show');

}


function showPreSave(sections) {
    var preview = $('#presavewindow');
    preview.html('');



    $.grep(sections, function (v, i) {

        var v2 = $(v).clone();

        $(v).find('select:not([multiple])').each(function () {
            var vselect = this;

            $(v2).find('select[name="' + $(vselect).attr('name') + '"]').val($(vselect).val()).trigger('change');

        });

        var title = $(v2).data('title') || '';


        if (!title) {
            var tabs = $(v2).parents('.tab-pane');
            if (tabs.length > 0) {
                title = $('a.nav-link[href="#' + tabs[0].id + '"]').html();
            }
        }

        preview.append('<div class="row"><div class="col-md-12"><h3 class="preview-title">' + title + '</h3></div></div>');

        v2.removeClass().appendTo('#presavewindow');

    })

    $('#presavewindow .m-scrollable').each(function (evt, elm) {
        debugger;
        $(elm).css('overflow', 'auto', 'important');
        $(elm).removeClass('ps');
    });

    $("#presavewindow :input").attr("disabled", true);
    $("#presavewindow :input").attr("disabled", true);

    $("#presavewindow .btn").remove();

    $('#modal_presave').modal('show');

}

changeSelect()

function changeSelect() {
    $('.selected-value').on('change', function () {
        var val = $(this).val()
        $(this).find('option').each(function (i, opt) {

            if ($(opt).val() === val) {
                $(opt).attr('selected', 'selected');
            }
            else
                $(opt).removeAttr('selected');

        });
    });
}

$('.btnpreview').click(function () {
    var selector = $(this).data('sections');

    if (selector)
        showPreview($(selector));
    else if ($('form#all .m-form__section')) {
        showPreview($('form#all .m-form__section'));
    } else {
        showPreview($('form .m-form__section'));
    }

})

$('.presaveClose').click(function () {
    //$(document).off('click', '#presaveSubit');
    //$('#presaveSubit').off('click');
    var preview = $('#presavewindow');
    preview.html('');
});


$('#presaveSubit').on('click', function () {
    debugger;
    deleteSeparatorNumber('.separator_number')
    $('.btnactuallysave')[0].click();
});

$('.btnsave').click(function (e) {
    var selector = $(this).data('sections');

    e.preventDefault();

    //$(document).off('click', '#presaveSubit');
    //$('#presaveSubit').off('click', '#presaveSubit');

    var btn = this;

    var cloned = $(this).clone();
    if ($(this).closest('.btnactuallysave')) {
        debugger;
        $(cloned).removeClass('btnsave').addClass('btnactuallysave');
        var element = document.createElement("div");
        $(element).css('display', 'none').append(cloned).appendTo($(this).closest('.m-form__actions'));
        //.append(cloned);
    }

    if (selector) {
        showPreSave($(selector));
    } else if ($('form#all .m-form__section')) {
        showPreSave($('form#all .m-form__section'));
    } else {
        showPreSave($('form .m-form__section'));
    }

})

function deleteSeparatorNumber(selector) {
    $(selector).each(function () {
        var $e = $(this);

        $e.data('raw_val', $e.val());
    });

    delete $.valHooks.text.get;
    delete $.valHooks.text.set;

    $(selector).each(function () {
        var $e = $(this);
        $e.val($e.data('raw_val'));
    });
}

function getLogText(row) {

    var name = (row.UpdatedDate ? row.UpdatedBy : row.CreatedBy);
    var date = (row.UpdatedDate ? moment(row.UpdatedDate).format('DD/MM/YYYY HH:mm') : moment(row.CreatedDate).format('DD/MM/YYYY HH:mm'));
    var isUpdate = row.UpdatedDate ? true : false;

    var result = '<div>';
    result += (isUpdate ? "Updated at <br/>" : "Created at <br/>")
    result += (date)

    if (name) {
        result += '<br/> by ' + name;
    }

    result += '</div>'
    return result;

}

function getLog(data, type, row) {

    var name = (row.UpdatedDate ? row.UpdatedBy : row.CreatedBy);
    var date = (row.UpdatedDate ? moment(row.UpdatedDate).format('DD/MM/YYYY HH:mm') : moment(row.CreatedDate).format('DD/MM/YYYY HH:mm'));
    var isUpdate = row.UpdatedDate ? true : false;

    var result = '<div>';
    result += (isUpdate ? "Updated at <br/>" : "Created at <br/>")
    result += (date)

    if (name) {
        result += '<br/> by ' + name;
    }

    result += '</div>'
    return result;

}

function renderAction(data, type, row) {

    //<a href="/${window.areaName}/${window.controllerName}/preview/${row.Id}" title="View" class="btn btn-secondary btn-sm"><i class="fa fa-search m--font-primary"></i></a>
    if (window.areaName) {
        return `<div class="btn-group">
                <a href="/${window.areaName}/${window.controllerName}/entry/${row.Id}" title="Edit" class="btn btn-secondary btn-sm"><i class="fa fa-pencil-alt m--font-primary"></i></a>
                <form action="/${window.areaName}/${window.controllerName}/delete/${row.Id}" method="POST" onsubmit="return deleteListing()"><button type="submit" title="Delete" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-trash-alt m--font-primary"></i></button></form>
                <a href="/${window.areaName}/${window.controllerName}/entry/${row.Id}?opt=clone" title="Clone" class="btn btn-secondary btn-sm"><i class="fa fa-clone m--font-primary"></i></a>
            </div>`;
    } else {
        return `<div class="btn-group">
                <a href="/${window.controllerName}/entry/${row.Id}" title="Edit" class="btn btn-secondary btn-sm"><i class="fa fa-pencil-alt m--font-primary"></i></a>
                <form action="/${window.controllerName}/delete/${row.Id}" method="POST" onsubmit="return deleteListing()"><button type="submit" title="Delete" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-trash-alt m--font-primary"></i></button></form>
                <a href="/${window.controllerName}/entry/${row.Id}?opt=clone" title="Clone" class="btn btn-secondary btn-sm"><i class="fa fa-clone m--font-primary"></i></a>
            </div>`;
    }
}

function defaultrenderAdminAction(data, type, row) {
    var result = '';
    if (window.areaName) {
        result = `<div class="btn-group">`;

        if (row.IsDeleted) {
            result += `<form action="/${window.areaName}/${window.controllerName}/RestoreDelete/${row.Id}?menu=${menuName}" method="POST" onsubmit="return restoreDelete()"><button type="submit" title="Restore" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-undo m--font-primary"></i></button></form>`;
        } else {
            result += `<a href="/${window.areaName}/${window.controllerName}/entry/${row.Id}" title="Edit" class="btn btn-secondary btn-sm"><i class="fa fa-pencil-alt m--font-primary"></i></a>
                <form action="/${window.areaName}/${window.controllerName}/delete/${row.Id}" method="POST" onsubmit="return deleteListing()"><button type="submit" title="Delete" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-trash-alt m--font-primary"></i></button></form>`;

            result += `<a href="/${window.areaName}/${window.controllerName}/entry/${row.Id}?opt=clone" title="Clone" class="btn btn-secondary btn-sm"><i class="fa fa-clone m--font-primary"></i></a>`
        }
        result += `</div>`;

        return result;
    } else {
        result = `<div class="btn-group">`;
        if (row.IsDeleted) {
            result += `<form action="/${window.controllerName}/RestoreDelete/${row.Id}" method="POST" onsubmit="return restoreDelete()"><button type="submit" title="Restore" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-undo m--font-primary"></i></button></form>`;
        } else {
            result += `<a href="/${window.controllerName}/entry/${row.Id}" title="Edit" class="btn btn-secondary btn-sm"><i class="fa fa-pencil-alt m--font-primary"></i></a>
                <form action="/${window.controllerName}/delete/${row.Id}" method="POST" onsubmit="return deleteListing()"><button type="submit" title="Delete" data-id="${row.Id}"  class="btn btn-secondary btn-sm"><i class="fa fa-trash-alt m--font-primary"></i></button></form>`;

            result += `<a href="/${window.controllerName}/entry/${row.Id}?opt=clone" title="Clone" class="btn btn-secondary btn-sm"><i class="fa fa-clone m--font-primary"></i></a>`;
        }

        result += `</div>`;

        return result;
    }
}

function restoreDelete() {
    if (!confirm("Apakah Anda yakin me-restore data ini ?")) {
        return false;
    }
}

function DatatableInit(elm, opt) {
    opt.tablename = isNotUndefined(opt.tablename) ? (opt.tablename + '') : '';

    //opt.fnCreatedRow = function (row, data, index) {
    //    $('td', row).eq(0).html(index + 1);
    //}
    $(elm).addClass('stripe');

    var tableName = isNotUndefined(opt.tablename) ? (opt.tablename) : '';
    var dtInstanceName = isNotUndefined(opt.tablename) ? ('spopDt' + opt.tablename) : 'spopDt';
    $(elm)
        .find('th')
        .each(function (i, v) {
            if ($(v).html().toLowerCase() === 'action') $(v).addClass('notexport');
        })

    if (opt.static) {
        window[dtInstanceName] = $(elm).DataTable({
            aLengthMenu: [
                [10, 25, 50, 100, 200, -1],
                [10, 25, 50, 100, 200, "All"]
            ],
            aaSorting: [],
            bLengthChange: false,
            columnDefs: opt.columnDefs,
            scrollX: opt.scrollX || false,
            colReorder: true,
            drawCallback: drawCallback
        });
    } else {
        window[dtInstanceName] = $(elm).DataTable({
            aLengthMenu: [
                [10, 25, 50, 100, 200, -1],
                [10, 25, 50, 100, 200, "All"]
            ],
            aaSorting: [],
            bLengthChange: false,
            bInfo: true,
            columnDefs: opt.columnDefs,
            serverSide: true,
            processing: true,
            colReorder: true,
            //stateSave: true,
            drawCallback: drawCallback,
            scrollX: opt.scrollX || true,
            ajax: {
                url: opt.url,
                method: 'POST',
                headers: {
                    accept: 'application/json'
                },
                data: function (data) {
                    var x = {};
                    ////debugger;
                    x.draw = data.draw;
                    x.length = data.length;
                    x.StartDate = $('#' + opt.tablename + 'dateFrom input[type="hidden"]').val();
                    x.EndDate = $('#' + opt.tablename + 'dateTo input[type="hidden"]').val();
                    x.status = $('#status-filter-option').val();
                    x.seriCode = $('#sericode-filter-option').val();
                    x.seriId = $('#seriid-filter-option').val();
                    x.start = data.start;
                    x.search = data.search;
                    x.order = data.order;
                    x.columns = data.columns;
                    x.dyamicSearch = window[tableName + 'dynfiltering'];


                    if (typeof (opt.preprocess) === 'function') {
                        return opt.preprocess(x);
                    }
                    
                    return x;
                },
            },
            searchDelay: 500

        });
    }

    if (isNotUndefined(opt.useNumberCol) && opt.useNumberCol) {

        window[dtInstanceName].on('draw.dt', function () {
            window[dtInstanceName].column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = (i + 1) + ((window[dtInstanceName].page.info().page * window[dtInstanceName].page.info().length) || 0);
            });
        }).draw();

        window[dtInstanceName].on('column-reorder', function (e, settings, details) {
            window[dtInstanceName].column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = (i + 1) + ((window[dtInstanceName].page.info().page * window[dtInstanceName].page.info().length) || 0);
            });


            window.localStorage.setItem(window.location + "#" + tableName + "order", JSON.stringify(window[dtInstanceName].colReorder.order()));


        });

    }

    //$('.dataTables_filter input[name="searchinput"]').prop("name", "search.value");

    //dtListing.on('order.dt search.dt', function () {
    //    dtListing.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1;
    //    });
    //}).draw();

    //GetData();

    $('#' + tableName + 'dateFrom input[type="hidden"], #' + opt.tablename + 'dateTo input[type="hidden"]').on('change', function () {
        window[dtInstanceName].ajax.reload();
    })


    var buttons = new $.fn.dataTable.Buttons(window[dtInstanceName], {
        buttons: [
            {
                extend: 'excel',
                text: 'Export Excel',
                className: 'exportExcel btn-primary m-btn m-btn--icon margin-bottom-5',
                filename: isNotUndefined(opt.filename) ? opt.filename : $('h3.m-subheader__title.m-subheader__title--separator').html(),
                title: '',
                exportOptions: {
                    format: {
                        body: function (data, row, column, node) {
                            return opt.useNumberCol && column == 0 ? row + 1 : data.replace(/(&nbsp;|<([^>]+)>)/ig, "")
                        }
                    },
                    columns: ':visible:not(.notexport)',
                    modifier: {
                        page: 'all',
                        length: 'All'
                    },
                    //columns: [0, 1, 2, 3, 4, 5]
                }
            }
        ]
    }).container().prependTo($('#' + tableName + 'buttons'));

    $('#searchinput').keyup(function () {
        window[dtInstanceName].search($(this).val()).draw();
    })
    $('#' + tableName + 'length').val(window[dtInstanceName].page.len());

    $('#status-filter-option').change(function () {
        window[dtInstanceName].ajax.reload();
    })

    $('#sericode-filter-option').change(function () {
        window[dtInstanceName].ajax.reload();
    })

    $('#seriid-filter-option').change(function () {
        window[dtInstanceName].ajax.reload();
    })

    $('#' + tableName + 'length').change(function () {
        window[dtInstanceName].page.len($(this).val()).draw();
    });

    var selectedStr = window.localStorage.getItem(window.location + "#" + tableName);
    var colOrder = window.localStorage.getItem(window.location + "#" + tableName + "order");

    var selectedCol = [];
    var orderCol = [];

    try {
        selectedCol = JSON.parse(selectedStr) || [];
    } catch (e) {

    }

    try {
        orderCol = JSON.parse(colOrder);
        window[dtInstanceName].colReorder.order(orderCol);
    } catch (e) {

    }

    columns.filter(function (obj) { return obj.name }).forEach(function (col) {

        var isSelected = selectedCol.filter(function (obj) { return obj.name === col.name }).length > 0;
        var checked = isSelected || selectedCol.length == 0 ? 'checked' : '';
        $('#' + tableName + 'columnshow .columnshowopt').append(
            `<label class="m-checkbox col-4">
                            <input type="checkbox" ${checked} value="${col.targets}" data-name="${col.name}">${col.label}
                            <span></span>
                        </label>`
        )
    });


    try {
        $('#' + tableName + 'columns').select2({
            placeholder: "Columns",
            closeOnSelect: false,
            width: '80%',
            data: columns.map(function (e) {
                return {
                    id: e.targets,
                    text: e.label
                }
            })
        })
    } catch (err) { console.log(err) }


    $("#columns").change(function (e) {
        var values = $(this).val();

        $.each(columns, function (i, v) {
            if (v.targets != '_all' && v.targets != columns.length - 2) {
                var column = window[dtInstanceName].column(v.targets);
                column.visible(values.includes(v.targets + ''));
            }
        })


    })


    $('#' + tableName + 'columns').change(function (e) {
        var values = $(this).val();
    })


    $('#' + tableName + 'columns').val(columns.filter(e => e.hide != true && typeof (e.label) !== 'undefined').map(function (e) { return e.targets })).trigger('change');


    $(function () {
        var tools = $('#' + tableName + 'tblf_tmp');
        columns.filter(function (t) { return t.data && t.searchable }).sort(function (a, b) {
            // Use toUpperCase() to ignore character casing
            const genreA = a.label.toUpperCase();
            const genreB = b.label.toUpperCase();

            let comparison = 0;
            if (genreA >= genreB) {
                comparison = 1;
            } else if (genreA <= genreB) {
                comparison = -1;
            }
            return comparison;
        }).forEach(function (col) {
            $('#' + tableName + 'tblf_tmp .table_filter_colopts').append(
                '<option value="' + col.data + '" data-type="' + (col.type || 'string') + '">' + col.label + '</option>'
            )
        })

        var selected = $('#' + tableName + 'columnshow .columnshowopt input[type=checkbox]').map(function (cb) {
            return {
                checked: this.checked,
                targets: $(this).val(),
                name: $(this).data('name')
            };
        });

        selected.each(function () {

            var column = window[dtInstanceName].column(this.targets);
            column.visible(this.checked);

        })


        var functFilter = window[tableName + 'addFilter'];
        if (typeof (functFilter) === 'function') functFilter();


        $('.btn:not([data-toggle="m-tooltip"]').each(function () {
            $(this).attr('data-toggle', "m-tooltip")//.attr('data-skin', "dark");
            $(this).attr('data-placement', 'bottom');
            $(this).attr('data-container', "body");
            if (!$(this).attr('data-original-title')) {
                //$(this).attr('data-original-title', $(this).attr('title') || $(this).text())
                if ($(this).attr('title') != undefined) {
                    $(this).attr('data-original-title', $(this).attr('title'))
                }
            }

            $(this).popover({
                delay: { show: 100, hide: 130 },
                trigger: 'focus',
                container: 'body'
            });
        });

        //var filtertemplate = $('#' + tableName +'table_filter_template').clone();
        //filtertemplate.removeAttr('id');

        //$('#' + tableName +'table_filters_container').append(filtertemplate);
    });

}

toolTipView()

function toolTipView() {
    var list = $('.m-portlet__nav')
    list.find('li').attr('data-toggle', 'tooltip')

    var listOption = list.find('li')

    if (listOption != undefined && listOption.length == 3 && !$(listOption).hasClass('notooltip')) {
        listOption.eq(0).attr('title', 'Edit')
        listOption.eq(1).attr('title', 'Delete')
        listOption.eq(2).attr('title', 'Clone')
    }

    //if (list.eq(i).find('li').find('i').hasClass('fa-pencil-alt')) {
    //    list.eq(i).find('li').attr('title', 'Edit')
    //} else if ($(this).find('li').find('i').hasClass('fa-trash-alt')) {
    //    list.eq(i).find('li').attr('title', 'Delete')
    //} else if ($(this).find('li').find('i').hasClass('fa-clone')) {
    //    list.eq(i).find('li').attr('title', 'Clone')
    //}    
}

$('[data-toggle="tooltip"]').tooltip({

});

function drawCallback(settings) {
    $('.btn:not([data-toggle="m-tooltip"]').each(function () {
        $(this).attr('data-toggle', "m-tooltip");//.attr('data-skin', "dark");

        $(this).attr('data-placement', 'bottom');

        $(this).attr('data-container', ".m-page--fluid");
        if (!$(this).attr('data-original-title')) {
            //$(this).attr('data-original-title', $(this).attr('title') || $(this).text())
            if ($(this).attr('title') != undefined) {
                $(this).attr('data-original-title', $(this).attr('title'))
            }
        }

        $(this).tooltip({
            delay: { show: 100, hide: 130 },
            trigger: 'hover',
            container: 'body'
        });
    });

}

function datatableNumber(round) {
    //return $.fn.dataTable.render.number(',', '.', round || 0);
    //return $.fn.dataTable.render.number(',', '.', round || 2);

    return function (data, type, row) {
        if (data) {
            if (round)
                return '<div>' + formatNumber(data.toFixed(round)) + '</div>';
            else
                return '<div>' + formatNumber(data) + '</div>';
        } else
            return null;
    }

}

function datatableBigNumber(data, type, row) {
    if (data) {
        return '<div>' + formatBigNumber(data) + '</div>';
    } else
        return null;

}

function defaultStatusRender(data, type, row) {

    return '<div>' + (row.IsDeleted ? "Deleted" : row.IsDraft ? "Draft" : "Saved") + '</div>'

}


function datatableDate(format) {
    format = typeof (format) === 'undefined' ? 'DD/MM/YYYY' : (format || 'DD/MM/YYYY');
    return function (data, type, row) {
        if (data)
            return '<div>' + isNotUndefined(data) ? moment(data).format(format) : '' + '</div>';
        else
            return null;

    }
}

function renderYear(data, type, row) {
    return data;
}

function datatableBool() {
    return function (data, type, row) {
        if (data)
            return '<div>' + (data) ? 'True' : 'False' + '</div>';
        else
            return null;
    }
}

function copyToClipboard(str) {
    const el = document.createElement('textarea');
    el.value = str;
    el.setAttribute('readonly', '');
    el.style.position = 'absolute';
    el.style.left = '-9999px';
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
};


jQuery.fn.extend({
    numberVal: function (value) {

        if (typeof (value) != 'undefined') {

            try {
                if (value !== "" && value != null && !isNaN(parseFloat(value))) {
                    var innumber = parseFloat(value);
                    //if (innumber <= Number.MAX_SAFE_INTEGER) {
                    $(this).val(formatNumber(value));
                    return this;
                }
                else {
                    $(this).val(null);
                    return this;
                }
            }
            catch (ex) {
                $(this).val(null);
                return this;
            }
        } else {
            var text = $(this).val() + '';
            var vall = parseFloat(text.replaceAll(',', ''));

            try {
                if (vall != null && !isNaN(vall)) {
                    return vall;
                } else
                    return null;
            }
            catch (ex) {
                return null;
            }
        }


    }
});


window.regex = {
    number: /^((?:-)((?:0)[.,][0-9]{1,19}|[1-9]{1,19}(?:[.,]\d{1,19})?)|\d{0,20}(?:[.,]\d{1,20})?)$/,
    //number: /^((?:-)[1-9][0-9]{0,19}|\d{0,20})(?:[.,]\d{1,10})?$/,
    integer: /^-?\d+$/
}

formatNumber = function (number, nullReturn) {
    if (Number(number) != NaN && typeof (number) !== 'undefined') {
        number += '';

        var x = number.split('.');

        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }

        return x1 + x2;
    }
    else {
        if (nullReturn) {
            return nullReturn;
        }
        else {
            return number;
        }
    }
}

String.prototype.replaceAll = function (search, replaceValue) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replaceValue);
};

replaceAll = function (text, search, replaceValue) {
    var target = text;
    return target.replace(new RegExp(search, 'g'), replaceValue);
};

$(document).on('change', '.numberInputMask', function (e) {
    var curval = $(this).val();
    var splited = curval.split('.');
    var min = curval.split('-');
    ////debugger;

    var maxval = $(this).data('maxvalue');
    if (splited.length <= 2 && splited[1] != '' && min.length <= 2 && ((min.length == 1 && min[0] != '') || (min.length == 2 && min[0] == '')) && min[1] != '' && min[1] != '0') {

        var curval = $(this).numberVal();

        if (maxval && parseFloat(maxval) < parseFloat(curval)) {
            $(this).numberVal((splited[0].replaceAll(',', '') + "").substr(0, (maxval + "").length));
        } else {
            $(this).numberVal(curval);
        }
    }
});

//var down = false;
$(document).on("keydown", ".numberInputMask", function (event) {

    //if (down) return;
    //down = true;

    var c = String.fromCharCode(event.keyCode);
    var isWordCharacter = c.match(/[A-Za-z,]/);
    var isBackspaceOrDelete = event.keyCode === 8 || event.keyCode === 46;
    //debugger;
    if (isWordCharacter) {
        event.preventDefault();
    }
});

//document.addEventListener('keydown', function () {


//    // your magic code here
//}, false);

$(document).ready(function () {
    $('form:not(.submitonenter) input:not([type=textarea])').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});



//auto complete
function autocomplete(inp, arr, opt) {
    opt = opt || {};
    /*the autocomplete function takes two arguments,
    the text field element and an array of possible autocompleted values:*/
    var currentFocus;
    /*execute a function when someone writes in the text field:*/
    inp.addEventListener("input", showOpt);
    inp.addEventListener("click", showOpt);

    function showOpt(e) {
        var a, b, i, val = this.value;
        /*close any already open lists of autocompleted values*/
        closeAllLists();
        //if (!val) { return false; }
        currentFocus = -1;
        /*create a DIV element that will contain the items (values):*/
        a = document.createElement("DIV");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        /*append the DIV element as a child of the autocomplete container:*/
        this.parentNode.appendChild(a);
        /*for each item in the array...*/
        for (i = 0; i < arr.length; i++) {
            /*check if the item starts with the same letters as the text field value:*/
            if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                /*create a DIV element for each matching element:*/
                b = document.createElement("DIV");
                /*make the matching letters bold:*/

                var text = typeof (opt.text) === 'function' ? opt.text(arr[i]) : arr[i]

                b.innerHTML = "<strong>" + text.substr(0, val.length) + "</strong>";
                b.innerHTML += text.substr(val.length);
                /*insert a input field that will hold the current array item's value:*/
                b.innerHTML += "<input type='hidden' value='" + (typeof (opt.value) === 'function' ? opt.value(arr[i]) : arr[i]) + "'/>";
                /*execute a function when someone clicks on the item value (DIV element):*/
                b.addEventListener("click", function (e) {
                    /*insert the value for the autocomplete text field:*/
                    inp.value = this.getElementsByTagName("input")[0].value;
                    $(inp).trigger('change');
                    /*close the list of autocompleted values,
                    (or any other open lists of autocompleted values:*/
                    closeAllLists();
                });
                a.appendChild(b);
            }
        }
    }

    /*execute a function presses a key on the keyboard:*/
    inp.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 13) {
            /*If the ENTER key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) x[currentFocus].click();
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "autocomplete-active":*/
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all autocomplete items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all autocomplete lists in the document,
        except the one passed as an argument:*/
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}

function getParticipants(callback) {

    $.ajax({
        url: '/equity/Delisting/CheckParticipant',
        method: "POST",
        data: { query: '' },
        success: function (data) {

            if (typeof (callback) === 'function') {
                callback(data);
            }
        }

    })

}


function getSecurities(callback, includes) {

    $.ajax({
        url: '/equity/ChangeSecNum/CheckSecCode',
        method: "POST",
        data: { query: '', includes: typeof (includes) !== 'undefined' ? encodeURIComponent(includes) : null },
        success: function (data) {
            console.log(data);

            if (typeof (callback) === 'function') {
                callback(data);
            }
        }

    })

}

function getSecuritiesByParticipantsId(param, callback) {

    $.ajax({
        url: '/equity/Delisting/CheckSecCode',
        method: "POST",
        data: { query: '', issuer: param },
        success: function (data) {
            console.log(data);

            if (typeof (callback) === 'function') {
                callback(data);
            }
        }

    })

}


function matchStart(term, text) {
    if (text.toUpperCase().indexOf(term.toUpperCase()) == 0) {
        return true;
    }

    return false;
}

function usematchStart() {

}


function addHtmlTableRow() {
    var selectedDate = document.getElementById("dateSelectedHoliday").value;
    var notes = $("textarea[name='Form.Notes']").val();
    var url = $("#RedirectTo").val();
    location.href = url + "?dateTemp=" + selectedDate
        + "&notes= " + notes;
}


$(document).ready(function () {

    //$('.btn').each(function () {
    //    $(this).attr('data-toggle', "m-tooltip");//.attr('data-skin', "dark");//.attr("data-delay",'\'{"show":1000, "hide":3000}\'');

    //    if (!$(this).attr('data-original-title')) {
    //        $(this).attr('data-original-title', $(this).attr('title') || $(this).text())
    //    }

    //    $(this).tooltip({
    //        delay: { show: 600, hide: 130 }
    //    });
    //});

});

//function sortSelect2(data) { return data.sort((a, b) => a.text.localeCompare(b.text)); }
function getExportLink(data, type, row) {
    if (window.areaName) {
        return `<div class="btn-group">
                <a href="/${window.areaName}/${window.controllerName}/export/${row.Id}" title="Export" class="btn btn-secondary btn-sm"><i class="fa fa-file-excel m--font-primary"></i></a>
            </div>`
    } else {
        return `<div class="btn-group">
                <a href="/${window.controllerName}/export/${row.Id}" title="Export" class="btn btn-secondary btn-sm"><i class="fa fa-file-excel m--font-primary"></i></a>
            </div>`
    }
}

//function getParticipants(callback) {

//    $.ajax({
//        url: '/equity/Delisting/CheckParticipant',
//        method: "POST",
//        data: { query: '' },
//        success: function (data) {
//            console.log(data);

//            if (typeof (callback) === 'function') {
//                callback(data);
//            }
//        }

//    })

//}

function sortSelect2(data) { return data.sort((a, b) => a.text.localeCompare(b.text)); }

Date.prototype.addWorkDays = function (days, cb) {
    if (isNaN(days)) {
        console.log("Value provided for \"days\" was not a number");
        return;
    }

    var system = $("#SystemSettingWorkDay :selected").val();
    var year = $("#YearSettingWorkDay :selected").val();
    var id = $("#IdSettinngWorkday").val();
    var url = '/settingworkday/GetHolidays?module=Equity&year=' + year;

    if (typeof (window.holidayDates) === 'undefined') {
        var date = this;
        $.ajax({
            url: url,
            method: 'POST',
            success: function (data) {
                if (Array.isArray(data)) {
                    try {
                        window.holidayDates = data.map(function (obj) {
                            return moment.utc(obj.HolidayDate);
                        });

                        var result = date;
                        var momentobj = moment(date).startOf('day');
                        var isholiday = true;
                        while (isholiday) {
                            momentobj = momentobj.add(1, 'days');
                            if (momentobj.day() == 0 || momentobj.day() == 6 || $.grep(window.holidayDates, function (obj) { return obj.startOf('day').diff(momentobj, 'day') == 0 }).length > 0) {

                                isholiday = true;
                            } else {
                                isholiday = false;
                            }
                        }

                        if (typeof (cb) === 'function') cb(momentobj.toDate());

                        return momentobj.toDate();

                    } catch (ex) {

                    }

                }
            }
        });
    } else {

        var result = this;
        var momentobj = moment(this).startOf('day');
        var isholiday = true;
        while (isholiday) {
            momentobj = momentobj.add(1, 'days');
            if (momentobj.day() == 0 || momentobj.day() == 6 || $.grep(window.holidayDates, function (obj) { return obj.startOf('day').diff(momentobj, 'day') == 0 }).length > 0) {

                isholiday = true;
            } else {
                isholiday = false;
            }
        }

        if (typeof (cb) === 'function') cb(momentobj.toDate());
        return momentobj.toDate();
    }
};

$('.useloading').click(function () {
    mApp.blockPage({
        overlayColor: '#000000',
        type: 'loader',
        state: 'success',
        message: 'Please wait...'
    })
});

function blockPage() {
    mApp.blockPage({
        overlayColor: '#000000',
        type: 'loader',
        state: 'success',
        message: 'Please wait...'
    })
}

function formatBigNumber(data) {
    var val = data.toString()
    val = val.replaceAll(',', '')

    var splitval = val.split('.')
    var l = splitval[0].length
    if (l > 21) {
        val = splitval[0].substring(0, 21)
        splitval[0] = val
    }

    var newVal = thousandSeparator(splitval[0])

    if (splitval.length > 1) {
        newVal = newVal + '.' + splitval[1]
    }

    return newVal
}

function thousandSeparator(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function ajaxCall(url,param,success) {
    $.ajax({
        url: url,
        type: 'POST',
        data: param,
        success: function (result) {
            success(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.error(xhr.status);
            console.error(thrownError);
        }
    });
}

window.delay = (function (key) {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();