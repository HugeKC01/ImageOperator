# ImageOperator

This repository contains two Windows Forms (.NET 8) C# projects for image processing and visualization:

- **MyFirst (Filter)**: Basic image operations and color channel visualization.
- **Fast Fourier Transform**: Fourier transform visualization and analysis.

---

## Projects

### 1. MyFirst (Filter)
A WinForms application for basic image processing tasks:
- Load, display, and resize images.
- Apply grayscale, sepia, Sobel, Prewitt, and average filters.
- Visualize individual RGB and CMYK color channels.
- View and save processed images in a separate output window.
- Output window supports saving the result, closing just the output, or exiting the whole app.

#### Usage
- Open the app and use the menu to load an image.
- Apply filters or channel operations from the menu.
- The processed image appears in a new window (Form2), where you can save or close it.

### 2. Fast Fourier Transform
A WinForms application for visualizing the Fourier transform of images:
- Load and display an image.
- Compute and display the phase and magnitude spectrum using 2D FFT.
- Reconstruct the image using inverse FFT.

#### Usage
- Open the app and load an image.
- Click "Forward FFT" to view the magnitude spectrum.
- Click "Inverse FFT" to reconstruct and view the image from its spectrum.

---

## Requirements
- .NET 8 SDK
- Windows OS (WinForms)

## Build & Run
1. Open the solution in Visual Studio 2022 or later.
2. Set either project as the startup project.
3. Build and run.

## License
This project is licensed under the MIT License.

---

## Credits
- Developed for educational purposes in image processing and computer vision coursework.
