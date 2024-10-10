# Testing notes

In order to run the Speckle tests you will need to set up your own Speckle project 
and change the Speckle project ID accordingly in `Config.cs`.

The suggested order to run tests is:

* IfcLoadTests.cs
* SpeckleTests.cs

# Known Issues
 
- Only tested and validated to work on Windows 
- Only 25 of the 27 test files pass the IfcLoadTests.
- Meshing is a huge bottleneck to performance 
