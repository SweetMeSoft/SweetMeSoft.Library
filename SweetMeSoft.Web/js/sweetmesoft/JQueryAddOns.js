jQuery.fn.extend({
    disable: function () {
        return this.each(function () {
            this.disabled = true;
        });
    },
    enable: function () {
        return this.each(function () {
            this.disabled = false;
        });
    },
    check: function (checked) {
        if (checked) {
            $(this).parent().addClass('checked');
        }
        else {
            $(this).parent().removeClass('checked');
        }
        return this.prop('checked', checked);
    },
    checkValidity: function () {
        return this[0].checkValidity();
    },
    initializeSelect: function () {
        return this.each(function () {
            const dropDown = $(this);
            let closeOnSelect;
            let allowClear;
            let isForModal = $('#modal').is(':visible');
            if (dropDown.attr('multiple') === undefined) {
                closeOnSelect = true;
            }
            else {
                allowClear = true;
            }
            dropDown.select2({
                theme: 'bootstrap-5',
                closeOnSelect: closeOnSelect,
                allowClear: allowClear,
                dropdownParent: isForModal ? $('#modal') : ''
            });
        });
    },
    toBlob: function () {
        const img = $(this);
        const imgElement = img[0];
        // Crear un objeto Canvas para dibujar la imagen
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');
        canvas.width = imgElement.width;
        canvas.height = imgElement.height;
        context.drawImage(imgElement, 0, 0);
        // Obtener el objeto Blob de la imagen dibujada en el Canvas
        canvas.toBlob(blob => {
            this.blob = blob;
        });
    }
});
