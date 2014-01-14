'use strict';
angular.module('Playground.competition-type-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .directive('integer', function() {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var INTEGER_REGEXP = /^\-?\d+$/;
                ctrl.$parsers.unshift(function(viewValue) {
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
    })
    .controller('CompetitionTypeAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, CompetitionTypeResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionType = {
            competitorType: 0
        };
        $scope.alerts = [];

        $scope.addCompetitionType = function () {
            CompetitionTypeResource.add($scope.competitionType,
                function (data, status, headers, config) {
                    $state.go('competition-types');
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding competition type' });
                }
            );
        };

        $scope.cancel = function () {
            $state.go('competition-types');
        }

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        // todo: add this to separate module or directive validation 
        $scope.getCssClasses = function (ngModelContoller) {
            return {
                error: ngModelContoller.$invalid && ngModelContoller.$dirty,
                success: ngModelContoller.$valid && ngModelContoller.$dirty
            };
        };

        $scope.showError = function (ngModelController, error) {
            return ngModelController.$error[error];
        };

        $scope.canSave = function (ngFormController) {
            return ngFormController.$valid && ngFormController.$dirty;
        };
    }]);