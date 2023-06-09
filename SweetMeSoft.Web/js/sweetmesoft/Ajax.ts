namespace SweetMeSoft {
    /**
     *
     * @param url
     * @param options
     * @param dropDowns
     */
    export function getOptions(options: OptionsSelect) {
        options = <OptionsSelect>(setDefaults(options, defaultsSelect));
        let extraText = '';
        get({
            url: options.url,
            data: options.data,
            showSuccess: false,
            successCallback: data => {
                for (let dropDown of options.dropDowns) {
                    let firstText = options.firstText;
                    let closeOnSelect = false;
                    let allowClear = false;
                    if (options.enable) {
                        dropDown.removeAttr('disabled');
                    }
                    dropDown.empty();
                    if (dropDown.attr('multiple') === undefined) {
                        dropDown.append('<option value="" disabled selected>' + firstText + '</option>');
                        closeOnSelect = true;
                    } else {
                        dropDown.append('<option value="" disabled>' + firstText + '</option>');
                        allowClear = true;
                    }

                    $.each(data,
                        (key, val) => {
                            extraText = '';
                            if (options.extraOption != null &&
                                options.extraOption !== '') {
                                extraText += ' data-' + options.extraOption + '="' + val[options.extraOption] + '"';
                            }

                            if (options.extraOption2 != null &&
                                options.extraOption2 !== '') {
                                extraText += ' data-' + options.extraOption2 + '="' + val[options.extraOption2] + '"';
                            }

                            if (options.extraOption3 != null &&
                                options.extraOption3 !== '') {
                                extraText += ' data-' + options.extraOption3 + '="' + val[options.extraOption3] + '"';
                            }

                            if (val != null && options.text == '') {
                                dropDown.append('<option value="' + val + '"' + extraText + '>' + val + '</option>');
                            } else {
                                if (val != null && options.text != undefined) {
                                    const route = options.text.split('.');
                                    let text = '';
                                    let copy = val;
                                    for (let item of route) {
                                        text = copy[item]
                                        copy = val[item]
                                    }

                                    let flag = options.isCountries ? 'data-content="<img src=\'https://flagsapi.com/' + val.code + '/flat/24.png\' style=\'margin-right: .7rem;\'>' + text + '"' : '';

                                    dropDown.append('<option ' + flag + ' value="' + val[options.internal] + '"' + extraText + '>' + text + '</option>');
                                }																					
                            }
                        });
                    if (options.urlValues != undefined && options.urlValues !== '') {
                        get({
                            url: options.urlValues,
                            successCallback: response => {
                                const array = [];
                                if (response != null) {
                                    for (let i = 0; i < response.length; i++) {
                                        array.push(response[i]['Id']);
                                    }
                                }
                                dropDown.val(array);
                                //M.FormSelect.init(dropDown, { dropdownOptions: { container: document.body } });
                                dropDown.change();
                            }
                        })
                    } else {
                        if (options.value != null && options.value !== 0 &&
                            options.value !== '') {
                            dropDown.val(options.value);
                            dropDown.change();
                        } else {
                            if (data.length === 1 && options.autoSelect) {
                                let uniqueOption = dropDown.find(':not([disabled]):first').val();
                                // @ts-ignore
                                dropDown.val(uniqueOption);
                                dropDown.change();
                            }
                        }
                        if (options.value != null && options.value !== 0 &&
                            options.value !== '') {
                            dropDown.val(options.value);
                            dropDown.change();
                        }
                    }

                    dropDown.selectpicker({
                        width: 'auto'
                    });
                    dropDown.selectpicker('refresh');
                }

                if (options.callback != undefined) {
                    options.callback(data);
                }
            }
        })
    }

    /**
     *
     * @param options
     */
    export function get(options: OptionsRequest) {
        on();
        options = <OptionsRequest>(setDefaults(options, defaultsRequest));
        $.ajax({
            url: options.url,
            data: options.data,
            traditional: true,
            type: 'GET',
            success: (response) => {
                handleAjaxSuccess(options, response);
            },
            error: (jqXhr) => {
                handleAjaxError(options, jqXhr)
            }
        });
    }

    /**
     *
     * @param options
     */
    export function post(options: OptionsRequest) {
        on();
        options = <OptionsRequest>(setDefaults(options, defaultsRequest));
        $.ajax({
            url: options.url,
            data: options.data,
            type: 'POST',
            success: response => {
                handleAjaxSuccess(options, response);
            },
            error: (jqXhr) => {
                handleAjaxError(options, jqXhr)
            }
        });
    }

    export function downloadFile(options: OptionsRequest) {
        on();
        options = <OptionsRequest>(setDefaults(options, defaultsRequest));
        var form = new FormData();
        for (let item of Object.keys(options.data)) {
            form.append(item, options.data[item]);
        }
        $.ajax({
            type: 'POST',
            url: options.url,
            processData: false,
            contentType: false,
            data: form,
            xhrFields: {
                responseType: 'blob'
            },
            success: (data) => {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);
                a.href = url;
                a.download = options.filename;
                a.click();
                window.URL.revokeObjectURL(url);

                handleAjaxSuccess(options, data);
            },
            error: (jqXhr) => {
                handleAjaxError(options, jqXhr)
            }
        });
    }

    export function uploadFile(options: OptionsRequest) {
        on();
        options = <OptionsRequest>(setDefaults(options, defaultsRequest));
        var form = new FormData();
        if (options.uploadControl != null && options.uploadControl != undefined) {
            const files = (options.uploadControl.get(0) as HTMLInputElement).files;
            if (files != null) {
                for (let i = 0; i < files.length; i++) {
                    form.append('files', files[i]);
                }
            } else {
                swal.fire('Error', 'There is no files selected.', 'error')
            }
        }

        for (const item of Object.keys(options.data)) {
            if (Array.isArray(options.data[item])) {
                for (const item2 of options.data[item]) {
                    form.append(item, item2);
                }
            } else {
                form.append(item, options.data[item]);
            }
        }

        $.ajax({
            type: 'POST',
            url: options.url,
            dataType: 'json',
            contentType: false,
            processData: false,
            data: form,
            success: (response) => {
                handleAjaxSuccess(options, response);
            },
            error: (jqXhr) => {
                handleAjaxError(options, jqXhr);
            }
        });
    }

    function handleAjaxError(options: OptionsRequest, jqXhr) {
        off()
        if (options.showError) {
            if (options.errorMessage === undefined || options.errorMessage === null || options.errorMessage === '') {
                console.error(jqXhr.responseJSON != undefined ? jqXhr.responseJSON.Detail : jqXhr.responseText)
                swal.fire('Error', jqXhr.responseJSON != undefined ? jqXhr.responseJSON.Title : jqXhr.responseText, 'error');
            } else {
                console.error(options.errorMessage)
                swal.fire('Error', options.errorMessage, 'error');
            }
        }

        if (options.errorCallback != undefined && options.errorCallback != null) {
            options.errorCallback(jqXhr);
        }
    }

    function handleAjaxSuccess(options: OptionsRequest, response: any) {
        if (options.showSuccess) {
            swal.fire({
                title: 'Great!',
                text: options.successMessage != '' ? options.successMessage : 'Request made successfully',
                icon: 'success',
                onAfterClose: () => {
                    if (options.successCallback != undefined) {
                        options.successCallback(response);
                    }
                }
            });
        } else {
            if (options.successCallback != undefined) {
                options.successCallback(response);
            }
        }

        off();
    }
}