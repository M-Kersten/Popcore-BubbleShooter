#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioToolbox.h>
#import <UIKit/UIKit.h>
#import "Vibration.h"

#define IPAD_DEVICE UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad

@interface Vibration ()
@end

@implementation Vibration
    #pragma mark - Vibrate
    // check whether vibrations are supported on device, i.e. is the device an ipad
    + (BOOL) supportsVibrations 
    {
        return !(IPAD_DEVICE);
    }
    // Vibrates the device softly
    + (void) vibrateShort
    {
        AudioServicesPlaySystemSoundWithCompletion(1519, NULL); 
    }
    // Vibrates the device 1 time more strongly
    + (void) vibrateStrong
    {
        AudioServicesPlaySystemSoundWithCompletion(1520, NULL); 
    }
@end

#pragma mark - "C"
extern "C" 
{ 
    bool _SupportsVibrations() 
    {
        return [Vibration supportsVibrations];
    }    
    void _VibrateShort() 
    {
        [Vibration vibrateShort];
    }
    void _VibrateStrong() 
    {
        [Vibration vibrateStrong];
    }/*
    void _VibrateSelect()
    {
        [Vibration vibrateSelect]
    }
    void _VibrateSimpleLight()
    {
        [Vibration vibrateSimpleLight]
    }
    void _VibrateSimpleMedium()
    {
        [Vibration vibrateSimpleMedium]
    }
    void _VibrateSimpleHard()
    {
        [Vibration _VibrateSimpleHard]
    }*/
}
