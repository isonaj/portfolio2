(function () {
    'use strict';

    angular
        .module('app')
        .controller('portfolioController', portfolioController);

    portfolioController.$inject = ['$scope', 'portfolioService']; 

    function portfolioController($scope, portfolioService) {
        portfolioService.getData()
            .success(function (data) {
                console.log('success');
                $scope.portfolio = data;
            });


        //$scope.portfolio = portfolioService.query();
    }
})();
