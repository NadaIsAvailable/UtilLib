// the following method is used to format the image data
// it is commented out here for reference
// example of rawData: 
// image1.jpg/Alt text for image 1
// image2.jpg/Alt text for image 2
// pass in a string of image data
// returns an array of image data or a single image data
// const formatImageData = (rawData, returnAll = true, indexReturned = 0) => {
//     let splitData = rawData.split('\n');
//     let imgData = [];
//     for (let i = 0, l = splitData.length; i < l; i++) {
//         imgData.push(splitData[i].split('/'));
//     }

//     if (returnAll) return imgData;
//     else return imgData[indexReturned];
// }
// import { formatImageData } from './uiHandlers.js';

let intervalId;
let curIndex = 0;
// ** path may need to be adjusted based on your project structure **
const imageDirPath = 'media/images/projects/';
const curImgList = formatImageData(rawData);
const imgCount = curImgList.length;

// Select the rotate buttons from the DOM with their IDs (prev-image and next-image)
const leftBtn = document.querySelector('#prev-image');
const rightBtn = document.querySelector('#next-image');

const rotateImage = (imgElement, dir = 'right') => {
    // Calculate the next index based on the direction
    let step = dir === 'left' ? 1 : -1;
    curIndex = (curIndex + step + imgCount) % imgCount;

    // Set the image source and alt text
    imgElement.src = `${imageDirPath}${curImgList[curIndex][0]}`;
    imgElement.alt = curImgList[curIndex][1];

    // Clear the previous interval and set a new one
    clearInterval(intervalId);
    intervalId = setInterval(rotateImage, 3000, imgElement, dir);
}

if (imgCount === 1) {
    // If there is only one image, hide the rotate buttons
    leftBtn.classList.add('hidden');
    rightBtn.classList.add('hidden');

    // Set the image to the only image
    const imgElement = document.querySelector('#project-image');
    imgElement.src = `${imageDirPath}${curImgList[0][0]}`;
    imgElement.alt = curImgList[0][1];
} else {
    // Show the rotate buttons
    leftBtn.classList.remove('hidden');
    rightBtn.classList.remove('hidden');

    // If there are multiple images, set the first image
    // and set the current index to the last image so that when
    // the rotation starts, it will show the first image
    curIndex = imgCount - 1;

    // and start the rotation
    rotateImage(document.querySelector('#project-image'));
}

// Add event listener to the rotate buttons
leftBtn.addEventListener('click', () => {
    // Show the next left image
    rotateImage(document.querySelector('#project-image'), 'right');
});

rightBtn.addEventListener('click', () => {
    // Show the next right image
    rotateImage(document.querySelector('#project-image'), 'left');
});