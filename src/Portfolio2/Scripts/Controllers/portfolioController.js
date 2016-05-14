(function () {
    'use strict';

    angular
        .module('app')
        .controller('portfolioController', portfolioController);

    portfolioController.$inject = ['$scope', 'portfolioService']; 

    function portfolioController($scope, portfolioService) {
        portfolioService.getData()
            .success(function (data) {
                $scope.portfolio = data;
            });

        $scope.class = function (col) {
            if (col == $scope.sortCol)
            {
                if ($scope.sortRev)
                    return 'fa fa-sort-desc';
                else
                    return 'fa fa-sort-asc';
            }
            else
                return '';
        };

        $scope.sort = function (col) {
            if (col == $scope.sortCol)
                $scope.sortRev = !$scope.sortRev;
            else {
                $scope.sortCol = col;
                $scope.sortRev = false;
            }
        };

        $scope.sort('Code');
        $scope.hideZeroes = true;

        $scope.filter = function () {
            return function (item) {
                return item.Units !== 0;
                if ($scope.hideZeroes)
                    return item.Units !== 0;
                else
                    return true;
            }
        };
    }
})();
