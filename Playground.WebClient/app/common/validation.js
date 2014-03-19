﻿'use strict';
angular.module('Playground.validation', [])
    .directive('integer', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var INTEGER_REGEXP = /^\-?\d+$/;
                ctrl.$parsers.unshift(function (viewValue) {
                    if (INTEGER_REGEXP.test(viewValue)) {
                        // it is valid
                        ctrl.$setValidity('integer', true);
                        return viewValue;
                    } else {
                        // it is invalid, return undefined (no model update)
                        ctrl.$setValidity('integer', false);
                        return undefined;
                    }
                });
            }
        };
    });