'use strict';
angular.module('Playground.imageupload', [])

    .controller('ImageUploadController', ['$http', '$scope', '$attrs', '$parse', '$interpolate', function ($http, $scope, $attrs, $parse, $interpolate) {
        $scope.changePicturePhase = 'initial';
        $scope.cropingArgs = null;

        this.uploadPicture = function (file, onUploadedCalback) {
            console.log('upload url ', $attrs.uploadUrl);
            console.log('uploading picture...');
            var formData = new FormData();
            if ($attrs.uploadUrl) {
                formData.append("image", file);
                formData.append("ID", $scope.objectId);
                $http(
                {
                    method: 'POST',
                    url: $attrs.uploadUrl,
                    data: formData,
                    headers: {
                        'Content-Type': undefined
                    },
                    transformRequest: angular.identity
                }).success(function (data, status, headers, config) {
                    $scope.changePicturePhase = 'crop';
                    onUploadedCalback(data);
                });
            }
            else {
                throw Error('no upload url specified');
            }
        };

        $scope.cropPicture = function () {
            if ($attrs.cropUrl) {
                if ($scope.cropingArgs != null) {
                    $scope.cropingArgs.id = $scope.objectId;
                    $http(
                    {
                        method: 'POST',
                        url: $attrs.cropUrl,
                        data: $scope.cropingArgs,
                    }).success(function (data, status, headers, config) {
                        $scope.croppingUrl = '';
                        $scope.imgSrc = data;
                        $scope.changePicturePhase = 'initial';
                        $scope.onImageCropped({ data: data });
                    });
                }
            }
            else {
                throw Error('No cropping url specified');
            }
        }

        $scope.$on("cropingSelectionChanged", function (event, args) {
            $scope.cropingArgs = args.coords;
        });

        $scope.$on("cropingSelectionReleased", function (event) {
            $scope.cropingArgs = null;
        });
    }])

    .constant('cropingConfig', {
        aspectRatio: 1,
        minWidth: 100,
        minHeight: 100,
        maxWidth: 300, 
        maxHeight: 300
    })

    .directive('imageUpload', ['cropingConfig', function (config) {
        return {
            restrict: 'E',
            templateUrl: 'app/templates/imageupload/image-upload.html',
            replace: true,
            scope: {
                objectId: '=',
                imgSrc: '=',
                uploadUrl: '@',
                cropUrl: '@',
                onImageCropped: '&'
            },
            controller: 'ImageUploadController',
            link: function (scope, element, attrs, imageUploadCtrl) {
                var fileEl = element.find('#fileupload'),
                    croppingEl = element.find('#croppingImg'),
                    myImg,
                    coords = {},
                    aspectRatio = attrs.aspectRatio ? parseInt(attrs.aspectRatio) : config.aspectRatio,
                    minWidth = attrs.minWidth ? parseInt(attrs.minWidth) : config.minWidth,
                    minHeight = attrs.maxHeight ? parseInt(attrs.maxHeight) : config.maxHeight,
                    maxWidth = attrs.maxWidth ? parseInt(attrs.maxWidth) : config.maxWidth,
                    maxHeight = attrs.maxHeight ? parseInt(attrs.maxHeight) : config.maxHeight;


                fileEl.bind('change', function (event) {
                    if (event.target.files[0]) {
                        imageUploadCtrl.uploadPicture(event.target.files[0], onFileUploaded);
                    }
                });

                var clear = function () {
                    if (myImg) {
                        myImg.next().remove();
                        myImg.remove();
                        myImg = undefined;
                    }
                };

                var onFileUploaded = function (url) {
                    clear();
                    croppingEl.after('<img />');
                    myImg = croppingEl.next();
                    myImg.attr('src', url);
                    $(myImg).Jcrop({
                        aspectRatio: aspectRatio,
                        minSize: [minWidth, minHeight],
                        maxSize: [maxWidth, maxHeight],
                        setSelect: [0, 0, minWidth, minHeight],
                        trackDocument: true,
                        onSelect: function (rect) {
                            scope.$emit("cropingSelectionChanged", { coords: rect });
                        },
                        onRelease: function (rect) {
                            scope.$emit("cropingSelectionReleased");
                        }
                    });
                };

                scope.$on('$destroy', clear);
            }
        };
    }]);
