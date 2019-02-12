# BookingApp

## Git workflow

Do not make changes directly to `master`. Create a new branch and open a pull request instead. Assign another developer to the PR and only merge once your changes have been reviewed.

Keep pull requests focused on a particular task or feature, long PRs are difficult to review. Also, keep commits short and focused. If the commit description seems too long it's probably a good idea to break up your code addition into multiple commits. Focused commits help identify issues in the long run and provide a better picture when looking at the project commit history.

## Milestones

* Property admin
* Reservations
* Statistics
* Clean-up

## Guidelines

Logically the app should be broken down into two layers: the __UI__ layer and the __Data__ layer. The UI should consist of controls that are relatively self-suficient and that don't hold data, but are initialized when first displayed. A screen or control should always reinitialize itself when the user navigates _to_ it (even navigating away from it and back). The Data layer should provide the data necessary for the control to reinitialize itself.

Have a separate canvas for large UI elements or elements with a lot of animations.

[Optimization tips for Unity UI](https://unity3d.com/how-to/unity-ui-optimization-tips)

## Packages, Libraries and Assets

* [Google Material Design package on asset store](https://assetstore.unity.com/packages/tools/particles-effects/google-material-design-47141) - google material design UI controls and animations
* [UI Tween](https://assetstore.unity.com/packages/tools/animation/ui-tween-38583) - the Google Material Design package is dependent on this

## Potential performance improvements

* improve writing: debounce data file write
* improve reading: use dictionaries keyed by reservation/room ID
* improve processing: use sorted lists
