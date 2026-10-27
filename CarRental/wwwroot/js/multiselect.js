const DDLforChosen = document.getElementById("selectedOptions");
const DDLforAvail = document.getElementById("availOptions");

//Switch the selected options from one drop down to another

function switchOptions(event, senderDDL, receiverDDL) {
    event.preventDefault(); //Prevents the browser from reloading

    const selectedOptions = Array.from(senderDDL.options).filter(option => option.selected);

    if (selectedOptions.length === 0) {
        alert("Nothing to move");
        return;
    }

    selectedOptions.forEach(option => {
        senderDDL.remove(option.index);
        receiverDDL.appendChild(option);
        option.selected = false; //This removes the selection
    });
}

//Event Handlers

function addOptions(event) {
    switchOptions(event, DDLforAvail, DDLforChosen);
}
function removeOptions(event) {
    switchOptions(event, DDLforChosen, DDLforAvail);
}

// Assign the event listeners
document.getElementById("btnArrowMinus").addEventListener("click", removeOptions);
document.getElementById("btnArrowPlus").addEventListener("click", addOptions);


//This next peace of code makes sure that even if nothing was selected options in the Chosen list will be selected, saved, and then passed to the selectedOptions array in C# 
document.getElementById("btnSubmit").addEventListener("click", () => {
    Array.from(DDLforChosen.options).forEach(option => {
        option.selected = true;
    });
});