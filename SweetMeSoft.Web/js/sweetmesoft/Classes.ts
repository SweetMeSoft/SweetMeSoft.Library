namespace SweetMeSoft {
    export function setDefaults(options: any, defaults: any) {
        return (<any>Object).assign({}, defaults, options);
    }

    export interface OptionsSelect {
        url: string;
        data?: string | Object | FormData;
        dropDowns: JQuery[];
        autoSelect?: boolean;
        callback?: (list: any) => void;
        enable?: boolean;
        extraOption?: string;
        extraOption2?: string;
        extraOption3?: string;
        firstText?: string;
        limitSubTextOption?: number;
        subTextOption?: string;
        text?: string;
        internal?: string;
        urlValues?: string;
        value?: number | string | string[];
        isCountries?: boolean;
    }

    export interface OptionsCropper {
        uploadControl: JQuery;
        imgControl: JQuery;
        callback?: (blob) => void;
    }

    export interface OptionsModal {
        title: string;
        type: 'view' | 'html'
        loadCallback?: Function;
        closeCallback?: Function;
        primaryCallback?: Function;
        cancelCallback?: Function;
        html?: string;
        viewUrl?: string;
        viewData?: string | Object;
        size?: 'small' | 'big';
        height?: 'auto' | '20%' | '30%' | '40%' | '50%' | '60%' | '70%' | '80%';
        primaryText?: string;
        cancelText?: string;
    }

    export interface OptionsTable {

        /**
         * Table to generate
         * @mandatory
         */
        table: JQuery;

        /**
         * Number of rows per page
         */
        rowsPerPage?: number;

        /**
         * Url to get the data
         */
        dataUrl?: string;

        /**
         * Params to get the data
         */
        dataParams?: Object;

        /**
         * Text 
         * @default No info
         */
        noDataText?: string;

        /**
         * Name of columns to hide. These would be written at lowercase
         * @default []
         */
        hiddenColumns?: string[];

        /**
         * Column name to default order. This would be written at lowercase
         * @default First column
         */
        defaultOrderColumn?: string;

        /**
         * It's ordered by ascendent or descendent
         * @default asc
         */
        defaultOrderType?: 'asc' | 'desc';

        /**
         * Name of columns to add. These would be written at lowercase
         * @default []
         */
        additionalColumns?: string[];

        /**
         * Allow you define if you want to show/hide the header
         * @default true
         */
        showHeader?: boolean;

        /**
         * Allow you define if you want to show/hide the footer
         * @default true
         */
        showFooter?: boolean;

        /**
         * Edit the format for specific columns
         * @default true
         */
        customColumns?: CustomColumn[];

        /**
         * Hieght of the table. Allow px, vn or auto
         * @default auto
         */
        height?: string;

        /**
         * Allow you define the buttons to show
         * @default []
         */
        buttons?: TableButton[];

        /**
         * Allow you define the buttons to show
         * @default []
         */
        onDblClick?: (rowData) => void;
    }

    interface TableButton {
        type: 'update' | 'delete' | 'custom';
        icon?: string;
        text?: string;
        color?: string;
        callback?: (rowData) => void;
    }

    interface CustomColumn {
        originalName: string;
        name: string;
        format: 'currency' | 'percentaje' | 'right' | 'image' | 'date';
        backgroundColor?: string;
    }

    export interface OptionsRequest {
        url: string;
        filename?: string;
        data?: string | Object;
        uploadControl?: JQuery;
        successCallback?: (response) => void;
        successMessage?: string;
        errorCallback?: (response) => void;
        errorMessage?: string;
        showError?: boolean;
        showSuccess?: boolean;
    }

    export interface OptionsConsecutive {
        documentId: number;
        businessUnitId?: number;
        serviceTypeId?: number;
        executionCityId?: number;
        companyId?: number;
        saveOp?: boolean;
        successEvent?: Function;
    }

    export interface ItemDropDown {
        id: string;
        text: string;
        callback: (item: Item) => void;
        hasDivider?: boolean;
    }

    export interface OptionsPostObject {
        /**
         * Complete url address for Service to call
         */
        url: string;

        /**
         * Object to save
         */
        object?: Object;

        /**
         * Method to call in case of success. Gets the response of the service
         * @param response
         */
        callback?: (response) => void;

        /**
         * Method to call in case of error. Gets the response of the service in string, status and the jquery object
         * @param response
         * @param status
         * @param jqXhr
         */
        errorCallback?: (response: string, status, jqXhr) => void;

        /**
         * Custom text for the popup initial validation when deleting an object
         */
        text?: string;

        /**
         * Custom text of response in case of success (200) and that doesn't have callback.
         */
        successMessage?: string;

        /**
         * Custom text of response in case of error (400, 500, etc) and that doesn't have callback.
         */
        errorMessage?: string;

        /**
         * If it's true, it will show a message if the response was success. This message will be taken from successMessage. In case of not defining it, it will show a default message.
         */
        showConfirmMessage?: boolean;

        /**
         * If it's true, it will show a message if the response was error. This message will be taken from errorMessage. In case of not defining it, it will show a default message.
         */
        showErrorMessage?: boolean;
    }

    export interface Item {
        id: number;
        divId: string;
        html: string;
        item: any;
    }

    export const defaultsSaveObject: OptionsPostObject = {
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

    export const defaultsDeleteObject: OptionsPostObject = {
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

    export const defaultsSelect = {
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
        value: 0,
        isCountries: false
    };

    export const defaultCropper = {
        callback: (blob) => { }
    }

    export const defaultsModal = {
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

    export const defaultsTable = {
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

    export const defaultsConsecutive = {
        documentId: 0,
        businessUnitId: 0,
        serviceTypeId: 0,
        executionCityId: 0,
        companyId: 1,
        saveOp: false,
        successEvent: (data: any) => {
        }
    };

    export const defaultsRequest = {
        url: '',
        filename: '',
        data: '',
        uploadControl: null,
        successCallback: (response: any) => {
        },
        errorCallback: (response: any) => {
        },
        errorMessage: '',
        successMessage: '',
        showError: true,
        showSuccess: true
    };

    export interface AutocompleteExtendedItem {
        id: string,
        text: string,
        img?: string
    }

    export interface DateTimePickerOptions {
        /**
         * Min date to allow select
         * @default null
         */
        minDate?: Date;
        /**
         * Max date to allow select
         * @default null
         */
        maxDate?: Date;
    }

    export const defaultDateTimePickerOptions: DateTimePickerOptions = {
        minDate: null,
        maxDate: null
    };
}
