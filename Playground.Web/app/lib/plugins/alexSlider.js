/******************************************************
* 
* Project name: Vega IT Sourcing Alex Slider - Version 1.6
* Date: 17.01.2013
* Author: Vega IT Sourcing Alex Slider by Aleksandar Gajic
* 
******************************************************/
// Script required
// jquery.js version 1.4.2 or above
//
// Script to initialize slider
// $('.slider').Slider();
//	
//	Minimum HTML for slider.
//	<div class="slider">      
//		<ul>            
//			<li>
//				<img src="image1.png" alt="image1" />
//			</li>
//			<li>
//				<img src="image2.png" alt="image2" />
//			</li>
//		</ul>
//	</div>
//
// Inside li tags you can put what ever you want, not just image tag.

(function ($) {
    $.fn.Slider = function (options) {
        var defaults = {
            prevBtnClass: 'prevBtn',    // Css class for previous button
            prevBtnText: 'Previous',    // Text for previous button
            nextBtnClass: 'nextBtn',    // Css class for next button
            nextBtnText: 'Next',        // Text for next button
            playBtnText: 'Play',        // Text for play button
            pauseBtnText: 'Pause',      // Text for pause button
            playBtnClass: 'playBtn',    // Css class for play button
            pauseBtnClass: 'pauseBtn',  // Css class for pause button
            showControls: true,         // Show next previous buttons
			wrapControls: false,		// It will wrap controls with ul list
			wrapControlsClass: 'slider-navigation',	// It will wrap controls with ul list
            speed: 600,                 // Speed for completing animation
            auto: true,                 // Auto rotate items in slider
            pause: 3000,                // Pause between two animations
            width: -1,                  // Set width for li tags in slider
            continues: true,			// After sliding all items, return to first, otherwise will stop at last element
            overedge: false,            // Animation effect to go over edge current item
            overedgePercentage: 12,     // Percentage of with/height to do overedge
            overedgeSpeed: 300,         // Speed of overedge animation
            autoresize: false,          // Auto risize slider elements
            orientationVertical: false, // If set to true it gonna slide verticaly otherwise it's gonna be horizontal slide animation
            verticalWrapper: 'veritcal-wrapper',    // Css class for vertical wrapper
            navigationWrapperClass: 'navigation-controls',  // Css class for navigation ul list
            navigationClass: 'thumbNav',    // Css class for navigation items	
            navigation: false,          // Show/hide navigation
            counter: false,             // Show/hide counter of items (1 of 5)
            fadeEffect: false,          // Animation is fade effect
            itemsToDisplay: 1,          // How many items are displayed
            disableOnClick: false,      // After clicking on next, previous, or some button on navigation auto sliding is turned off
            playPause: false,           // Show/hide play pause buttons
            timeToStartPlay: 0,         // Time after clicking play to start animation
            galleryClass: 'preview-gallery',    // Css class for gallery with tumbnail images
            galleryImageThumb: '',      // For thumbnail images in galery will be used first img tag in slider li tags. If you have thumbnail images with same image name but with some suffix then you can say what suffix is. Like in umbraco, all images have theirs thumbnails with _thumb suffix
            showGallery: false,         // Show/hide thumbnail images for navigation, it takes first img tag in li tag-s of slider
            counterText: ' of ',        // Text between counter numbers
			classWrapper: 'inner-wrapper', // Additional wrapper class name
			setWrapper: true, 			// Set additional wrapper	
            complete: function () { },   // For additional scripts to execute after comleting initialization of 			
			changedItem: null,	// For additional scripts to execute after item is changed. Example: function ($item) { alert($item.attr('class')); }; //$item is new displayed element
			itemClicked: null,	// For additional scripts to execute on item is clicked. Example: function (e, $item) { alert($item.attr('class')); },
			dragging: false,
			changeOrientation: false, // For changing orienation of slides movement,
			responsive: false // For enabling responsive behaviour
        };

        options = $.extend(defaults, options);

        this.each(function () {
            var s, realItemsCount, currentIndex, leftright,
                animationStarted = false, obj = $(this), initialization = $(obj).hasClass('initialized'),
                disableAutoForce = false, nextButtonSelector = 'a.' + options.nextBtnClass,
                previousButtonSelector = 'a.' + options.prevBtnClass, playButtonSelector = 'a.' + options.playBtnClass,
                pauseButtonSelector = 'a.' + options.pauseBtnClass,
                navigationSelector = '.' + options.navigationWrapperClass + ' ul.' + options.navigationClass + ' li a',
                currentNavigationSelector = '.' + options.navigationWrapperClass + ' ul.' + options.navigationClass + ' li a.cur',
                gallerySelector = '.' + options.galleryClass + ' ul li a',
                currentGallerySelector = '.' + options.galleryClass + ' ul li a.cur',
                slectroForGalleryAndNavigation = '', itemsInSlider, timeout, h, i, w, t, ot, 
                counter, heightBody, newHeight, html, cssClass = '', imageUrl, length, extension,
                litag, selector, index, selectedIndex, selectedItem, verticalWrapperSelector = '.' + options.verticalWrapper, 
				$liTag, cssCalss, orientation = {}, overEdgeorientation = {}, movingLastFirstorientation = {}, itemSize, property,
				$first, $last, sel, startX, inResize, aspectRatio;

            if (options.navigation) {
                slectroForGalleryAndNavigation = navigationSelector;
            }

            if (options.showGallery) {
                if (slectroForGalleryAndNavigation !== '') {
                    slectroForGalleryAndNavigation += ', ';
                }

                slectroForGalleryAndNavigation += gallerySelector;
            }

            if (options.playPause) {
                options.auto = false;
            }
						
            function checkContinues() {										
                if (!options.continues) {					
                    if (options.showControls && (currentIndex === 1 || currentIndex === realItemsCount)) {
						clearTimeout(timeout);
						disableAutoForce = true;						
                        if (currentIndex === realItemsCount) {							
                            $(nextButtonSelector, obj).addClass('disabled');
                            $(previousButtonSelector, obj).removeClass('disabled');
                        }

                        if (currentIndex === 1) {							
                            $(previousButtonSelector, obj).addClass('disabled');
                            $(nextButtonSelector, obj).removeClass('disabled');
                        }
                    } else {
						if (options.showControls) {
							$(nextButtonSelector, obj).removeClass('disabled');
							$(previousButtonSelector, obj).removeClass('disabled');
						}
					}
                }
            }

            function setHeightOfActiveTag(pos, stopAnimation) {
                if (options.autoresize) {
                    if (stopAnimation) {
                        $(obj).height($('ul:first > li', obj).eq(pos).height());
                    }

                    newHeight = $('ul:first > li', obj).eq(pos).height();
                    $(obj).animate(
						{ height: newHeight + 10 },
						options.speed * 3 / 4);
                }
            }
			
			function animationFinished(movingLastFirstorientation, property, itemSize, itemChanged) {
				animationStarted = false; 							
				if (t === itemsInSlider && options.continues) {
					t = options.itemsToDisplay;
					movingLastFirstorientation[property] = t * itemSize * (-1) + 'px';
					$('ul:first', obj).css(movingLastFirstorientation);
				}
				
				if (t === 0 && options.continues) {
					t = itemsInSlider - options.itemsToDisplay;
					movingLastFirstorientation[property] = t * itemSize * (-1) + 'px';
					$('ul:first', obj).css(movingLastFirstorientation);
				}
								
				if (options.changedItem) {
					options.changedItem($('ul:first > li', obj).eq(t));
				}
			}
			
			function doAnimation(newMargin, direction) {				
				itemSize = w;
				property = 'margin-left';
				if (options.orientationVertical) {					
					property = 'margin-top';
					itemSize = h;
				}
				
				orientation[property] = newMargin;
				if (options.overedge) {
					overEdgeorientation[property] = newMargin;
					orientation[property] = newMargin - (leftright * itemSize * (options.overedgePercentage / 100))
				}											
				
				 $('ul:first', obj).animate(orientation, options.speed,
					function () {
						if (options.overedge) {
							$('ul:first', obj).animate(overEdgeorientation, options.overedgeSpeed, function () {
									animationFinished(movingLastFirstorientation, property, itemSize);
								});
						} else { 
							animationFinished(movingLastFirstorientation, property, itemSize);
						}
					});
			}
				
			function animationAdditionalTasks(clicked) {
				cssClass = $('ul:first > li', obj).eq(t).attr('class');					
				
				if (options.navigation) {
					selector = $('ul:first > li', obj).eq(t).attr('class');					
					if (selector) {
						selector = selector.replace('ng-scope ', '');
						selector = selector.replace('clone ', '');
					}

					$(navigationSelector, obj).each(function () {
						if ($(this).hasClass(selector)) {
							$(this).addClass('cur');
						} else {
							$(this).removeClass('cur');
						}
					});
				}

				if (options.showGallery) {
					selector = $('ul:first > li', obj).eq(t).attr('class');
					selector = selector.replace('clone ', '');
					$(gallerySelector, obj).each(function () {
						if ($(this).hasClass(selector)) {
							$(this).addClass('cur');
						} else {
							$(this).removeClass('cur');
						}
					});
				}
				
				if (options.counter) {
					currentIndex = parseInt(cssClass.split('tagorder-')[1], 10);
					html = currentIndex + ' of ' + realItemsCount;
					if ($(obj).next().hasClass('sliderInfo')) {
						$(obj).next().html(html);
					}					
				}
				
				if (clicked && options.auto) {
                    clearTimeout(timeout);                    
					if (!options.disableOnClick) {
						timeout = setTimeout(
							function () {
								commenceAnimation('forward', false);
							}, 
							options.pause
						);
					}
                }
				
				setHeightOfActiveTag(t);
				checkContinues();
			}
			
            function commenceAnimation(direction, clicked, disableClick) {
                if (!disableClick) {
                    if (s != 1 && !animationStarted) {
                        animationStarted = true;
                        switch (direction) {
                        case 'forward':
                            leftright = 1;							
							if (options.changeOrientation) {
								t -= 1;
								if (t < 0) {
									t = options.fadeEffect ? itemsInSlider - 1 : itemsInSlider;
								}
							} else {
								t += 1;							
								if (t > itemsInSlider || options.fadeEffect && t === itemsInSlider) {
									t = options.continues && !options.fadeEffect ? options.itemsInSlider : 0;
								}       
							}							
                                                        
                            break;
                        case 'previous':
                            leftright = 0 - 1;      
							if (options.changeOrientation) {
								t += 1;							
								if (t > itemsInSlider || options.fadeEffect && t === itemsInSlider) {
									t = options.continues && !options.fadeEffect ? options.itemsInSlider : 0;
								}  
							} else {
								t -= 1;
								if (t < 0) {
									t = options.fadeEffect ? itemsInSlider - 1 : itemsInSlider;
								}
							}
                            break;
                        default:
                            break;
                        }
						
						animationAdditionalTasks(clicked);
                        if (options.fadeEffect) {
                            $('ul:first > li', obj).fadeOut(options.speed);
                            $('ul:first > li', obj).eq(t).fadeIn(options.speed, function () {
                                animationStarted = false;
                            });
                        } else {
							if (options.orientationVertical) {
								doAnimation(t * h * (-1), leftright);
							} else {
								doAnimation(t * w * (-1), leftright);
							}
                        }											
                    }
                }
				
                if (options.auto && direction == 'forward' && !clicked && $(obj)) {
                    timeout = setTimeout(function () {
                        commenceAnimation('forward', false, false);
                    }, options.speed + options.pause);
                }
            }
			
			function resizeingSlider() {
				animationStarted = true;
				inResize = true;
				if(this.resizeTO) clearTimeout(this.resizeTO);
				this.resizeTO = setTimeout(function() {
					$(this).trigger('resizeEnd');
				}, 300);
				
				w = $(obj).outerWidth();
				if (!options.orientationVertical) {
					w = w / options.itemsToDisplay;
				}
				
				aspectRatio = $('ul:first > li img:first', obj).width() / $('ul:first > li img:first', obj).height();
				$('ul:first > li', obj).width(w);
				$('ul:first > li img', obj).width(w);
				
				h = w / aspectRatio;
				
				if (options.orientationVertical) {
					h = h / options.itemsToDisplay;
				}
				
				$('ul:first > li img', obj).height(h);
				$('ul:first > li', obj).height(h);
				$(obj).css('height', h);
				
				if (options.orientationVertical) {
					$(verticalWrapperSelector, obj).height(h);
					$('ul:first', obj).height(h * s);
					$('ul:first', obj).width(w);
				} else {
					$('ul:first', obj).height(h);
					$('ul:first', obj).width(w * s);
				}
				
				property = 'margin-left';
				newMargin = t * w * (-1)
				if (options.orientationVertical) {					
					property = 'margin-top';
					itemSize = h;
					newMargin = t * h * (-1);
				}
				
				$('ul:first', obj).css(property, newMargin);
			}

            if (options.autoresize) {
                heightBody = $('ul:first > li:first', obj).height();
				if (!options.responsive) {
					$(obj).css('height', heightBody + 'px');
				}
            }

			// Initialize slider
			itemsInSlider = s = $('ul:first > li', obj).length;
			realItemsCount = s;			
            if (!initialization) {
                $(obj).addClass('initialized');

                counter = 1;
                $('ul:first > li', obj).each(function () {
                    $(this).addClass('tagorder-' + counter);
                    counter += 1;
                });
				
				t = 0;
				if (options.continues && !options.fadeEffect) {
					for (i = 0; i < options.itemsToDisplay; i++) {					
						$first = $('ul:first > li', obj).eq(2 * i);
						$last = $('ul:first > li', obj).eq(s - 1);
						
						$('ul:first', obj).append(
							$('<li/>', {								
								html: $first.html()
							}).addClass('clone ' + $first.attr('class'))
						);
						
						$('ul:first', obj).prepend(
							$('<li/>', {								
								html: $last.html()
							}).addClass('clone ' + $last.attr('class'))
						);
					}
					
					s+= 2 * options.itemsToDisplay;
					t = options.itemsToDisplay;
					itemsInSlider += options.itemsToDisplay; 
				}
				
				if (options.dragging) {
					$('img', obj).bind('dragstart', function(e) { 
						e === null ? e.preventDefault() : e = window.event.preventDefault();						 
					});
					
					$('li' , obj).mousedown(function (e) {
						if (e === null) e = window.event;
						 if (e.button === 1 && window.event !== null || e.button === 0) {
							startX = e.clientX;
						 }
					}).mouseup(function (e) {
						if (e === null) e = window.event;
						 if (e.button == 1 && window.event != null || e.button == 0) {
							if (Math.abs(startX - e.clientX) > ($(this).width() / 100 * 15)) { //If its more then 15% dragged than move slide
								commenceAnimation(startX - e.clientX > 0 ? 'forward' : 'previous', true, false);
							}
						 }
					});
				}
				
                h = $('ul:first > li:first', obj).outerHeight(); // Calculating height of slider
                w = $('ul:first > li:first', obj).outerWidth(); // Calculating width of slider item		
                if ($('ul:first > li img', obj).length) {
                    $('ul:first > li img', obj).each(function () {
                        $('ul:first > li img', obj).eq(0).load(function () {
                            h = $('ul:first > li', obj).height();
                            if ($(obj).outerHeight < h) {
                                $(obj).outerHeight(h);
                            }
                        });
                    });
                } else {
                    h = $('ul:first li', obj).outerHeight();
                    $(obj).outerHeight(h);
                }
				
                if (options.width !== -1) {
                    w = options.width;                                       
                }
				
				if (options.responsive) {
					aspectRatio = $('ul:first > li img:first', obj).width() / $('ul:first > li img:first', obj).height();
					property = 'margin-left';
					if (!options.orientationVertical) {
						w = w / options.itemsToDisplay;
					}
					
					itemSize = w;
					$('ul:first > li img', obj).width(w);
					$('ul:first > li', obj).width(w);
					h = w / aspectRatio;
					if (options.orientationVertical) {
						h = h /options.itemsToDisplay;
						property = 'margin-top';
						itemSize = h;
					}
					$('ul:first > li img', obj).height(h);
					$('ul:first > li', obj).height(h);
					
					$(obj).css('width', '100%');
					$(obj).css('height', h);
					
					$(window).bind('resizeEnd', function() {
						animationStarted = false;
						inResize = false;
					});
					
					$(window).resize(function() {
						if (!animationStarted || inResize) {
							resizeingSlider();
						} else {
							$('ul:first', obj).stop();
							animationStarted = false;
							animationFinished(movingLastFirstorientation, property, itemSize);
							resizeingSlider();
						}
					});
				}

                if (options.showGallery) {
                    html = '<div class="' + options.galleryClass + '">';
                    html += '<ul>';

                    imageUrl = '';
                    for (i = 0; i < realItemsCount; i++) {
						cssClass = i === 0 ? (i + 1) + ' cur' : (i + 1) + '';
						
						if (options.continues && !options.fadeEffect) {
							$liTag = $('ul:first > li', obj).eq(i + options.itemsToDisplay);
						} else {						
							$liTag = $('ul:first > li', obj).eq(i);
						}
						
                        imageUrl = $liTag.children('img:first').attr('src');
                        if (options.galleryImageThumb !== '') {
                            index = imageUrl.lastIndexOf('.');
                            length = imageUrl.length;
                            extension = imageUrl.substr(index + 1, length);
                            imageUrl = imageUrl.substr(0, index) + options.galleryImageThumb + '.' + extension;
                        }

                        html += '<li><a href="javascript:;" class="tagorder-' + cssClass + '"><img src="' + imageUrl + '" alt="image"/></a></li>';
                    }
					
                    html += '</ul>';
                    html += '</div>';
                    $(obj).append(html);
                }

                if (options.showControls && s != 1) {					
					html = options.wrapControls ? '<ul class="' + options.wrapControlsClass + '"><li>' : '';
					
					html += '<a href="javascript:void(0);" class="' + options.prevBtnClass;

                    if (!options.continues) {
                        html += ' disabled';
                    }
					
                    html += '"><span>' + options.prevBtnText + '</span></a>';
					html += options.wrapControls ? '</ll><li>' : '';
                    html += ' <a href="javascript:void(0);" class="' + options.nextBtnClass + '"><span>' + options.nextBtnText + '</span></a>';
					html += options.wrapControls ? '</ll></ul>' : '';
                    $(obj).append(html);
                }

                if (options.playPause && s != 1) {
                    html = '<a href="javascript:void(0);" class="' + options.playBtnClass + '"><span';
                    html += ' >' + options.playBtnText + '</span></a>';
                    html += ' <a href="javascript:void(0);" class="' + options.pauseBtnClass + '"><span>' + options.pauseBtnText + '</span></a>';
                    $(obj).append(html);
                }

                if (options.playPause) {
                    $(pauseButtonSelector, obj).hide();

                    $(playButtonSelector, obj).click(function () {
                        $(playButtonSelector, obj).hide();
                        $(pauseButtonSelector, obj).show();
                        options.auto = true;
                        setTimeout(function () {
                            commenceAnimation('forward', false);
                        }, options.timeToStartPlay);
                    });

                    $(pauseButtonSelector, obj).click(function () {
                        $(pauseButtonSelector, obj).hide();
                        $(playButtonSelector, obj).show();
                        options.auto = false;
                        clearTimeout(timeout);
                    });
                }

                if (options.showControls) {
                    $(nextButtonSelector, obj).click(function () {
                        if (!$(this).hasClass("disabled")) {
                            commenceAnimation('forward', true, false);
                        }

                        if (options.playPause) {
                            $(pauseButtonSelector, obj).hide();
                            $(playButtonSelector, obj).show();
                        }
                    });

                    $(previousButtonSelector, obj).click(function () {
                        if (!$(this).hasClass("disabled")) {
                            commenceAnimation('previous', true, false);
                        }

                        if (options.playPause) {
                            $(pauseButtonSelector, obj).hide();
                            $(playButtonSelector, obj).show();
                        }
                    });
                }

                if (options.counter) {
                    html = '<p class="sliderInfo"> 1 of ' + realItemsCount + '</p>';
                    $(obj).after(html);
                }               

                if (options.navigation && realItemsCount > 1) {                    
                    html = '<div class="' + options.navigationWrapperClass + '">';
                    html += '<ul class="' + options.navigationClass + '">';
                    for (i = 0; i < realItemsCount; i += 1) {
						cssClass = i === 0 ? (i + 1) + ' cur' : (i + 1) + '';
                        html += '<li><a href="javascript:;" class="tagorder-' + cssClass + '"><span>' + (i + 1) + '</span></a></li>';
                    }

                    html += '</ul>';
                    html += '</div>';
                    $(obj).append(html);
                }

                if (options.autoresize) {
                    obj.height(h);
                } else {
                    $('ul:first', obj).height(h);
                    $('ul:first > li', obj).height(h);
                }

                //obj.css("overflow", "hidden");
				if (!options.responsive) obj.css('position', 'relative');
				if (!options.fadeEffect) {
					if (options.orientationVertical) {
						$('ul:first', obj).css('margin-top', t * h * (-1)+ 'px');
					} else {
						$('ul:first', obj).css('margin-left', t * w * (-1)+ 'px');
					}
				}                

                if (options.fadeEffect) {
                    $('ul:first', obj).css('width', w).find('> li').css('position', 'absolute').hide().eq(t).show();
                } else {
                    if (options.orientationVertical) {
                        $('ul:first', obj).css('height', s * h).wrap('<div class="' + options.verticalWrapper + '"/>');
                        $(verticalWrapperSelector, obj).css('height', h * options.itemsToDisplay + 'px').css('overflow', 'hidden');
                    } else {
						if (!options.responsive) {
						$(obj).css('width', w * options.itemsToDisplay + 'px');
						}
                        $('ul:first', obj).css('width', s * w).find('> li').css('float', 'left');					
                    }
                }

                if (options.setWrapper) {
					$('ul:first', obj).wrap('<div class="' + options.classWrapper + '"/>');
				}

                $(slectroForGalleryAndNavigation, obj).click(function () {
                    if (!$(this).hasClass('cur')) {
                        if (options.playPause) {
                            $(pauseButtonSelector, obj).hide();
                            $(playButtonSelector, obj).show();
                        }

                        if (!animationStarted) {                            
							animationStarted = true;
							$(navigationSelector, obj).removeClass('cur');
							$(gallerySelector, obj).removeClass('cur');
							
							selectedItem = $(this).attr('class');
							
							$(this).addClass('cur');
							sel = 'ul:first > li.' + selectedItem + ':not(".clone")';
							selectedIndex = $(sel, obj).index();							
																																							
							t = selectedIndex;														
							if (options.autoresize) {
								setHeightOfActiveTag(t);
							}

							animationAdditionalTasks(true);
							if (options.fadeEffect) {
								$('ul:first > li', obj).fadeOut(options.speed);
								$('ul:first > li', obj).eq(t).fadeIn(options.speed, function () {
										animationStarted = false;
									});
							} else {
								if (options.orientationVertical) {
									doAnimation(t * h * (-1), leftright);
								} else {
									doAnimation(t * w * (-1), leftright);
								}
							}                          
                        }
                    }
                });
				
				if (options.itemClicked) {
					$('ul:first > li', obj).click(function(e) {
						options.itemClicked(e, $(this), t);
					});					
				}

                if (options.autoresize) {
                    setHeightOfActiveTag(t, true);
                }

                if (options.auto) {
                    timeout = setTimeout(function () {
                        commenceAnimation('forward', false);
                    }, options.pause);
                }

				animationAdditionalTasks(false);				
                options.complete();
            }
        });
    };
})(jQuery);