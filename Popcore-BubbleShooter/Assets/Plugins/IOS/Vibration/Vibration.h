#import <Foundation/Foundation.h>

@interface Vibration : NSObject

	#pragma mark - Vibrate
	+ (BOOL)    supportsVibrations;
	+ (void)    vibrateShort;
	+ (void)    vibrateStrong;
@end
