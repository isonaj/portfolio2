(function () {
    'use strict';

    angular
        .module('app')
        .controller('portfolioController', portfolioController);

    portfolioController.$inject = ['$scope', 'portfolioService']; 

    function portfolioController($scope, portfolioService) {
        $scope.portfolio = portfolioService.query();
    }
})();
