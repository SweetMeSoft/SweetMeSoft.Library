var SweetMeSoft;
(function (SweetMeSoft) {
    function setDefaults(options, defaults) {
        return Object.assign({}, defaults, options);
    }
    SweetMeSoft.setDefaults = setDefaults;
    SweetMeSoft.defaultsSaveObject = {
        url: '',
        object: {},
        callback: undefined,
        errorCallback: undefined,
        text: '',
        successMessage: 'It saved successfully.',
        errorMessage: 'We cannot save it.',
        showConfirmMessage: true,
        showErrorMessage: true
    };
    SweetMeSoft.defaultsDeleteObject = {
        url: '',
        object: {},
        callback: undefined,
        errorCallback: undefined,
        text: 'Are you sure?',
        successMessage: 'It was deleted successful.',
        errorMessage: 'It could not be deleted.',
        showConfirmMessage: true,
        showErrorMessage: true
    };
    SweetMeSoft.defaultsSelect = {
        url: '',
        data: {},
        dropDowns: [],
        autoSelect: true,
        callback: () => {
        },
        enable: false,
        extraOption: '',
        extraOption2: '',
        extraOption3: '',
        firstText: 'Select',
        limitSubTextOption: 0,
        subTextOption: '',
        text: 'description',
        internal: 'id',
        urlValues: '',
        value: 0
    };
    SweetMeSoft.defaultCropper = {
        callback: (blob) => { }
    };
    SweetMeSoft.defaultsModal = {
        title: 'Modal',
        type: 'html',
        loadData: {},
        loadCallback: undefined,
        closeCallback: undefined,
        primaryCallback: undefined,
        cancelCallback: undefined,
        html: 'You need to define html code',
        viewUrl: '',
        gridOptions: {},
        size: 'small',
        height: 'auto',
        primaryText: 'Thanks',
        cancelText: 'Cancel'
    };
    SweetMeSoft.defaultsTable = {
        table: null,
        rowsPerPage: 10,
        dataUrl: '',
        dataParams: {},
        noDataText: 'No info',
        hiddenColumns: [],
        additionalColumns: [],
        defaultOrderColumn: '',
        defaultOrderType: 'asc',
        showHeader: true,
        showFooter: true,
        customColumns: [],
        buttons: [],
        height: 'auto',
        onDblClick: (rowData) => { }
    };
    SweetMeSoft.defaultsConsecutive = {
        documentId: 0,
        businessUnitId: 0,
        serviceTypeId: 0,
        executionCityId: 0,
        companyId: 1,
        saveOp: false,
        successEvent: (data) => {
        }
    };
    SweetMeSoft.defaultsRequest = {
        url: '',
        filename: '',
        data: '',
        callback: (response) => {
        },
        errorCallback: (response) => {
        },
        errorMessage: '',
        successMessage: '',
        showError: true,
        showSuccess: true
    };
    SweetMeSoft.defaultDateTimePickerOptions = {
        minDate: null,
        maxDate: null
    };
})(SweetMeSoft || (SweetMeSoft = {}));
