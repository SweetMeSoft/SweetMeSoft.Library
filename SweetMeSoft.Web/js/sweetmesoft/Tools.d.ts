declare namespace SweetMeSoft {
    function on(): void;
    function off(): void;
    function capitalizeFirstLetter(string: string): string;
    /**
     *
     * @param date
     * @param withTime
     */
    function getFormatedDate(date: Date, format: string, withTime?: boolean): string;
    function isValidDate(date: any): boolean;
    function getUrlParameter(sParam: any): string;
    function generateCropper(options: OptionsCropper): void;
    function generateModal(options: OptionsModal): void;
    function generateTable(options: OptionsTable): void;
}
