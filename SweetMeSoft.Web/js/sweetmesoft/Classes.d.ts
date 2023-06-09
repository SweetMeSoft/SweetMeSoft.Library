declare namespace SweetMeSoft {
    export function setDefaults(options: any, defaults: any): any;
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
        callback?: (blob: any) => void;
    }
    export interface OptionsModal {
        title: string;
        type: 'view' | 'html';
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
        onDblClick?: (rowData: any) => void;
    }
    interface TableButton {
        type: 'update' | 'delete' | 'custom';
        icon?: string;
        text?: string;
        color?: string;
        callback?: (rowData: any) => void;
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
        successCallback?: (response: any) => void;
        successMessage?: string;
        errorCallback?: (response: any) => void;
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
        callback?: (response: any) => void;
        /**
         * Method to call in case of error. Gets the response of the service in string, status and the jquery object
         * @param response
         * @param status
         * @param jqXhr
         */
        errorCallback?: (response: string, status: any, jqXhr: any) => void;
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
    export const defaultsSaveObject: OptionsPostObject;
    export const defaultsDeleteObject: OptionsPostObject;
    export const defaultsSelect: {
        url: string;
        data: {};
        dropDowns: any[];
        autoSelect: boolean;
        callback: () => void;
        enable: boolean;
        extraOption: string;
        extraOption2: string;
        extraOption3: string;
        firstText: string;
        limitSubTextOption: number;
        subTextOption: string;
        text: string;
        internal: string;
        urlValues: string;
        value: number;
        isCountries: boolean;
    };
    export const defaultCropper: {
        callback: (blob: any) => void;
    };
    export const defaultsModal: {
        title: string;
        type: string;
        loadData: {};
        loadCallback: any;
        closeCallback: any;
        primaryCallback: any;
        cancelCallback: any;
        html: string;
        viewUrl: string;
        gridOptions: {};
        size: string;
        height: string;
        primaryText: string;
        cancelText: string;
    };
    export const defaultsTable: {
        table: any;
        rowsPerPage: number;
        dataUrl: string;
        dataParams: {};
        noDataText: string;
        hiddenColumns: any[];
        additionalColumns: any[];
        defaultOrderColumn: string;
        defaultOrderType: string;
        showHeader: boolean;
        showFooter: boolean;
        customColumns: any[];
        buttons: any[];
        height: string;
        onDblClick: (rowData: any) => void;
    };
    export const defaultsConsecutive: {
        documentId: number;
        businessUnitId: number;
        serviceTypeId: number;
        executionCityId: number;
        companyId: number;
        saveOp: boolean;
        successEvent: (data: any) => void;
    };
    export const defaultsRequest: {
        url: string;
        filename: string;
        data: string;
        uploadControl: any;
        successCallback: (response: any) => void;
        errorCallback: (response: any) => void;
        errorMessage: string;
        successMessage: string;
        showError: boolean;
        showSuccess: boolean;
    };
    export interface AutocompleteExtendedItem {
        id: string;
        text: string;
        img?: string;
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
    export const defaultDateTimePickerOptions: DateTimePickerOptions;
    export {};
}
