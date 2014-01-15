'use strict';
angular.module('Playground.competition-type.validation', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.alert',
    'Playground.validation'])
    .directive('playersperteam', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var INTEGER_REGEXP = /^\-?\d+$/,
                    MIN_VALUE = 2,
                    MAX_VALUE = 20
                function validateTeamPlayers(viewValue) {
                    var valid = true,
                        minValue = scope.$eval(attrs.minvalue) || MIN_VALUE,
                        maxValue = scope.$eval(attrs.maxvalue) || MAX_VALUE;

                    var condition = scope.$eval(attrs.condition)
                    if (condition) {
                        valid = false;
                        if (INTEGER_REGEXP.test(viewValue)) {
                            var value = parseInt(viewValue);
                            if (value >= minValue && value <= maxValue) {
                                valid = true;
                            }
                        }
                    }
                    ctrl.$setValidity('playersperteam', valid);
                    return valid ? viewValue : undefined;
                }

                ctrl.$parsers.push(validateTeamPlayers);
                ctrl.$formatters.push(validateTeamPlayers);
                scope.$watch(attrs.condition, function () {
                    ctrl.$setViewValue(ctrl.$viewValue);
                });
            }
        };
    });