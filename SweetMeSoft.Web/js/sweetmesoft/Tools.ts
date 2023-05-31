namespace SweetMeSoft {

    export function on() {
        const overlay = document.getElementById('overlay');
        if (overlay != null) {
            overlay.style.display = 'block';
            onQuantity++;
        }
    }

    export function off() {
        if (onQuantity > 0) {
            onQuantity--;
            if (onQuantity === 0) {
                const overlay = document.getElementById('overlay');
                if (overlay != null) {
                    overlay.style.display = 'none';
                }
            }
        }
    }

    export function capitalizeFirstLetter(string: string): string {
        return string.charAt(0).toUpperCase() + string.slice(1);
    }

    /**
     *
     * @param date
     * @param withTime
     */
    export function getFormatedDate(date: Date, format: string, withTime?: boolean): string {
        const offset = new Date().getTimezoneOffset();
        date.setTime(date.getTime() - (offset * 60 * 1000));

        const year = date.getFullYear().toString();
        let month = (date.getMonth() + 1).toString();
        let day = date.getDate().toString();

        if (date.getDate() < 10) {
            day = '0' + day;
        }

        if (date.getMonth() + 1 < 10) {
            month = '0' + month;
        }

        if (!withTime) {
            return format.replace('yyyy', year).replace('MM', month).replace('dd', day)
            //return monthsAbreviated[date.getMonth()] + ' ' + day + ', ' + year;
        } else {
            let hour = date.getHours().toString();
            let minutes = date.getMinutes().toString();
            if (date.getHours() < 10) {
                hour = '0' + hour;
            }

            if (date.getMinutes() < 10) {
                minutes = '0' + minutes;
            }

            return format.replace('yyyy', year).replace('MM', month).replace('dd', day).replace('HH', hour).replace('mm', minutes)
        }
    }

    export function isValidDate(date): boolean {
        return date && Object.prototype.toString.call(date) === "[object Date]" && !isNaN(date);
    }

    export function getUrlParameter(sParam): string {
        var sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam && sParameterName[1] !== undefined) {
                return decodeURIComponent(sParameterName[1])
            }
        }

        return '';
    }

    export function generateCropper(options: OptionsCropper) {
        let cropper: Cropper;
        options.uploadControl.on('change', event => {
            swal.fire({
                title: 'Crop image',
                allowOutsideClick: false,
                html: '<img id="imgUploadedImage" src="" style="width: 100%; height: 100%;"/>',
                onOpen: () => {
                    $('#imgUploadedImage').attr('src', URL.createObjectURL((event.target as HTMLInputElement).files[0]));
                    cropper = new Cropper(($('#imgUploadedImage')[0] as HTMLImageElement), { initialAspectRatio: 1, aspectRatio: 1, modal: true, autoCropArea: 1 })
                },
                onClose: () => {
                    options.imgControl.attr('src', cropper.getCroppedCanvas().toDataURL());
                    cropper.getCroppedCanvas().toBlob(blob => {
                        options.callback(blob);
                    })
                }
            });
        });
    }

    export function generateModal(options: OptionsModal) {
        options = ((setDefaults(options, defaultsModal)) as OptionsModal);

        const modal = new bootstrap.Modal(document.getElementById('modal'), { backdrop: true })
        const body = $('#divModalBody');
        const title = $('#txtModalTitle');
        const btnModalPrimary = $('#btnModalPrimary');
        const btnModalCancel = $('#btnModalCancel');
        btnModalPrimary.text(options.primaryText);
        btnModalCancel.text(options.cancelText);
        body.html('');
        title.text(options.title);

        switch (options.type) {
            case 'view':
                on();
                body.load(options.viewUrl, options.viewData, () => {
                    if (options.loadCallback != undefined) {
                        options.loadCallback()
                        off();
                    } else {
                        off();
                    }
                });
                break;
            case 'html':
                body.html(options.html);
                break;
        }

        if (options.primaryCallback != undefined) {
            btnModalPrimary.show();
            btnModalPrimary.unbind('click')
            btnModalPrimary.on('click', event => {
                options.primaryCallback()
                modal.hide()
            });
        }

        if (options.cancelCallback != undefined) {
            btnModalCancel.show();
            btnModalCancel.unbind('click')
            btnModalCancel.on('click', event => {
                options.cancelCallback()
            });
        }

        if (options.size === 'big') {
            $('.modal-dialog').css('max-width', '80%');
            $('.modal-dialog').css('height', options.height);
        }

        modal.show();
    }

    export function generateTable(options: OptionsTable) {
        options = ((setDefaults(options, defaultsTable)) as OptionsTable);
        const regex = /^[a-zA-Z]/;
        SweetMeSoft.get({
            url: options.dataUrl,
            data: options.dataParams,
            showSuccess: false,
            successCallback: data => {
                const id = options.table.attr('id');
                options.hiddenColumns.forEach((column, index) => {
                    options.hiddenColumns[index] = column.toLowerCase();
                })
                let columns: DataTables.ColumnDefsSettings[] = []
                let index = 0;
                if (data.length > 0) {
                    const keys = Object.keys(data[0]);
                    for (const key of keys) {
                        const customFormat = options.customColumns.find(model => model.originalName.toLowerCase() == key.toLowerCase());
                        columns.push({
                            targets: index,
                            data: key,
                            title: capitalizeFirstLetter(key),
                            visible: !options.hiddenColumns.includes(key.toLowerCase()),
                            className: customFormat != undefined && (customFormat.format == 'currency' || customFormat.format == 'right') ? 'dt-body-right' : '',
                            createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {
                                if (customFormat != undefined) {
                                    if (customFormat.backgroundColor != undefined) {
                                        $(cell).css('background-color', customFormat.backgroundColor);
                                    }

                                    switch (customFormat.format) {
                                        case 'currency':
                                            if (cellData > 0) {
                                                $(cell).css('color', 'green')
                                            }
                                            else if (cellData < 0) {
                                                $(cell).css('color', 'red')
                                            }
                                            break;
                                    }
                                }

                                return data;
                            },
                            render: (data, type, row, meta) => {
                                if (customFormat != undefined) {
                                    switch (customFormat.format) {
                                        case 'currency':
                                            const dollar = new Intl.NumberFormat('en-US', {
                                                style: 'currency',
                                                currency: 'USD',
                                                minimumFractionDigits: 2,
                                            });

                                            if (+data < 0) {
                                                $(row).css('background-color', '#ffbaba')
                                            } else {
                                                $(row).css('background-color', '#d7fcdf')
                                            }

                                            return dollar.format(data);
                                        case 'image':
                                            if (data == null || data == '') {
                                                return '<img src="/images/default-product.png" class="rounded-circle" style="height:40px; width: 40px;"/>';
                                            }

                                            return '<img src="' + data + '" class="rounded-circle" style="height:40px; width: 40px;"/>';
                                    }
                                }

                                return data;
                            }
                        });
                    }

                    for (const i in data) {
                        const keys = Object.keys(data[i]) as string[]
                        for (const key of keys) {
                            const date = new Date(data[i][key]);
                            if (data[i][key] != null && isValidDate(date) && !$.isNumeric(parseFloat(data[i][key])) && typeof data[i][key] != 'boolean' && !regex.test((data[i][key]).charAt(0))) {
                                data[i][key] = SweetMeSoft.getFormatedDate(date, 'yyyy-MM-dd', true)
                            }
                        }
                    }

                    index++;
                } else {
                    data = [];
                    columns = [{
                        targets: index,
                        data: options.noDataText,
                        title: options.noDataText
                    }]

                    index++;
                }

                for (const additionalColumn of options.additionalColumns) {
                    columns.push({
                        targets: index,
                        data: additionalColumn,
                        title: capitalizeFirstLetter(additionalColumn)
                    })

                    index++;
                }

                let indexButton = 0;
                let callbacks = [];
                if (options.buttons.length > 0) {
                    columns.push({
                        targets: index,
                        data: '',
                        title: 'Actions',
                        className: 'action-row',
                        render: (data, type, row) => {
                            let htmlButtons = '';
                            for (let button of options.buttons) {
                                switch (button.type) {
                                    case 'update':
                                        htmlButtons += '<a id="btnTable' + indexButton + '" class="btn btn-primary btn-table"><i class="bi-pencil-fill icn-table"></i></a>';
                                        break;
                                    case 'delete':
                                        htmlButtons += '<a id="btnTable' + indexButton + '" class="btn btn-danger btn-table"><i class="bi-trash3-fill icn-table"></i></a>';
                                        break;
                                    case 'custom':
                                        htmlButtons += '<a id="btnTable' + indexButton + '" class="btn btn-table" style="background-color: ' + button.color + '"><i class="bi-' + button.icon + ' icn-table"></i></a>';
                                        break;
                                }

                                callbacks.push({
                                    id: indexButton,
                                    button: button,
                                    row: row
                                })
                                indexButton++;
                            }

                            return htmlButtons;
                        }
                    })

                    index++;
                }

                if ($.fn.dataTable.isDataTable(options.table)) {
                    const table = options.table.DataTable();
                    table.destroy();
                    options.table.html('');
                }

                const indexColumn = columns.findIndex(model => model.data.toString().toLowerCase() == options.defaultOrderColumn);

                let table = options.table.DataTable({
                    data: data,
                    columns: columns,
                    scrollX: true,
                    scrollY: options.height,
                    retrieve: true,
                    paging: options.showFooter,
                    info: options.showFooter,
                    drawCallback: settings => {
                        const buttons = $('.btn-table');
                        buttons.off('click');
                        buttons.each((index, button) => {
                            $(button).on('click', () => {
                                const id = $(button).attr('id').replace('btnTable', '');
                                const callback = callbacks.find(model => model.id == id);
                                if (callback.button.type == 'delete') {
                                    swal.fire({
                                        title: 'Delete item',
                                        text: 'Are you sure you want to delete this item? This action cannot be undone',
                                        icon: 'warning',
                                        showCancelButton: true,
                                        cancelButtonText: 'No',
                                        confirmButtonText: 'Yes'
                                    }).then(result => {
                                        if (result.value) {
                                            callback.button.callback(callback.row);
                                        }
                                    });
                                } else {
                                    callback.button.callback(callback.row);
                                }
                            })
                        });
                    },
                    searching: options.showHeader,
                    pageLength: options.rowsPerPage,
                    order: [[indexColumn == -1 ? 0 : indexColumn, options.defaultOrderType]],
                    language: {
                        lengthMenu: 'Show _MENU_ registers per page',
                        zeroRecords: 'Nothing found',
                        info: 'Page _PAGE_ of _PAGES_',
                        infoEmpty: 'No available info',
                        infoFiltered: '(filtrado desde _MAX_ total registros)',
                        search: 'Search',
                        paginate: {
                            first: 'First',
                            last: 'Last',
                            next: 'Next',
                            previous: 'Previous'
                        },
                    },
                })

                options.table.off('dblclick');
                options.table.on('dblclick', 'tr', function () {
                    var data = table.row(this).data();
                    options.onDblClick(data);
                });

                $("[name='" + id + "_length']").removeClass('form-select');

                for (const callback of callbacks) {
                    $('#btnTable' + callback.id).on('click', event => {
                        if (callback.button.type == 'delete') {
                            swal.fire({
                                title: 'Delete item',
                                text: 'Are you sure you want to delete this item? This action cannot be undone',
                                icon: 'warning',
                                showCancelButton: true,
                                cancelButtonText: 'No',
                                confirmButtonText: 'Yes'
                            }).then(result => {
                                if (result.value) {
                                    callback.button.callback(callback.row);
                                }
                            });
                        } else {
                            callback.button.callback(callback.row);
                        }
                    });
                }
            }
        })
    }
}