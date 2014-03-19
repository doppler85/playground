'use strict';
angular.module('Playground.user-list', ['ngResource', 'ui.router']).
controller('UserListController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
'UserResource',
function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.users = {};

    $scope.loadUsers = function (page) {
        UserResource.users({
            page: page,
            count: 5
        },
            function (data, status, headers, config) {
                $scope.users = data;
            }
        );
    }

    $scope.onUserPageSelect = function (page) {
        $scope.loadUsers(page);
    }

    $scope.loadUsers(1);
}]);;