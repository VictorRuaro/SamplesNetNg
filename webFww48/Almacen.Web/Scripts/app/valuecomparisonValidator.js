$.validator.unobtrusive.adapters.add('valuecomparison', ['property', 'comparison', 'notvalidatewhenvalue'], function (options) {
    options.rules['valuecomparison'] = options.params;
    options.messages['valuecomparison'] = options.message;
});

$.validator.addMethod('valuecomparison', function (value, element, parameters) {
    
    var comparison = parameters.comparison;
    var notvalidatewhenvalue = parameters.notvalidatewhenvalue;

    comparison = (comparison == null ? '' : comparison).toString();

    var actualElement = $(element).attr('id');

    //debugger

    var container = actualElement.split('__');

    var otherProperty = parameters.property;

    if ($(container).length > 1)
    {
        var name = parameters.property;
        var prename = container[0];
        otherProperty = prename + '__' + name;
    }

    var controlType = $("input[id$='" + otherProperty + "']").attr("type");
    var otherValue = {};

    if (controlType == "checkbox" || controlType == "radio") {
        var control = $("input[id$='" + otherProperty + "']:checked");
        otherValue = control.val();

        if (otherValue == undefined) {
            var valor = $("input[id$='" + otherProperty + "']").prop('checked');
            if (valor !== undefined) {
                otherValue = valor;
            }
        }

    } else {
        otherValue = $("input[id$='" + otherProperty + "']").val();
    }

    var actualValue = $(element).val();
    //select of enum ValueComparison from Almacen.Web.CustomAnnotations
    var isValid = true;

    if ($.trim(notvalidatewhenvalue).toLowerCase() === $.trim(otherValue).toLocaleLowerCase()) {
        return true;
    }
        if (actualValue != null && actualValue != '') {


            switch (comparison) {
                case 'IsEqual':// IsEqual
                    if ($.trim(actualValue).toLowerCase() === $.trim(otherValue).toLocaleLowerCase()) {

                        return !isValid;
                    }
                    return isValid;
                    break;

                case 'IsNotEqual': // IsNotEqual
                    if ($.trim(actualValue).toLowerCase() !== $.trim(otherValue).toLocaleLowerCase()) {

                        return !isValid;
                    }
                    return isValid;
                    break;
                case 'IsGreaterThan':// IsGreaterThan
                    if ($.isNumeric(actualValue) && $.isNumeric(otherValue)) {
                        if (parseInt(actualValue) > parseInt(otherValue)) {

                            return isValid;
                        }
                    }
                    return !isValid;
                    break;
                case 'IsGreaterThanOrEqual':// IsGreaterThanOrEqual
                    if ($.isNumeric(actualValue) && $.isNumeric(otherValue)) {
                        if (parseInt(actualValue) >= parseInt(otherValue)) {

                            return isValid;
                        }
                    }
                    return !isValid;
                    break;
                case 'IsLessThan':// IsLessThan
                    if ($.isNumeric(actualValue) && $.isNumeric(otherValue)) {
                        if (parseInt(actualValue) < parseInt(otherValue)) {

                            return isValid;
                        }
                    }
                    return !isValid;
                    break;
                case 'IsLessThanOrEqual':// IsLessThanOrEqual
                    //var x = otherProperty;
                    //var xy = actualElement;
                    if ($.isNumeric(actualValue) && $.isNumeric(otherValue)) {

                        if (parseInt(actualValue) <= parseInt(otherValue)) {

                            return isValid;
                        }
                    }
                    return !isValid;
                    break;
                default:
                    break;
            }
        }
    

    return true;
});