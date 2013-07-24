//Update the division types when a division checkbox is checked/unchecked
function UpdateDivDropdown(CheckBoxID, MaxPageCount)
{
	//get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	if(window.spanArrayGlobal!= null)
	 { 
	
	  spanArrayObjects = window.spanArrayGlobal;
	 
	 }
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }
     

	if(document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled==true)
	{
	  document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled=false;
	  
	  //If a division name textbox is required and disabled, enable it
	  document.getElementById('txtDivName'+(CheckBoxID.split('newdiv')[1])).disabled = false;
	}
	else
	{
	 document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).disabled=true;
	 //Disable the named division textbox
	 if(document.getElementById('selectDivType'+(CheckBoxID.split('newdiv')[1])).value.indexOf('!')==0)
	   document.getElementById('txtDivName'+(CheckBoxID.split('newdiv')[1])).disabled = true;
	 
	 //update the subsequent divs
	 var index=CheckBoxID.split('newdiv')[1];
	 var i = spanArray.indexOf(index);

	    if (i == 0) {
	        document.getElementById('selectDivType' + index).value = 'Main';

	        //		while(document.getElementById('selectDivType'+spanArray[i]).disabled==true)
	        while (i < spanArray.length) {
	            i++;
	            if (document.getElementById('selectDivType' + spanArray[i])) {
	                document.getElementById('selectDivType' + spanArray[i]).value = 'Main';
	                document.getElementById('newDivType' + spanArray[i]).checked = false;
	                document.getElementById('newDivType' + spanArray[i]).disabled = true;

	            }

	        }

	    } else {
	        var j = i - 1;
	        var valToSet = 'Main';

	        while (!(document.getElementById('selectDivType' + spanArray[j]))) {
	            j--;
	        }

	        valToSet = document.getElementById('selectDivType' + spanArray[j]).value;

	        var k = i;

	        //set the page division type of all pages till the start of the next div
	        while (document.getElementById('selectDivType' + spanArray[k]).disabled == true && k < spanArray.length) {
	            if (document.getElementById('selectDivType' + spanArray[k]))
	                document.getElementById('selectDivType' + spanArray[k]).value = valToSet;
	            k++;
	        }
	    }
	}

}

//Change all subsequent division types when one div type is changed. Also update the named division type textboxes as appropriate
function DivisionTypeChanged(selectID,MaxPageCount)
{
   //Get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	//Get the array from the global variable if previously assigned
	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	 //else set the array values here
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }

	var currID = selectID.split('selectDivType')[1];
   // var i=parseInt(currID)+1;
    var i = spanArray.indexOf(currID)+1;
	var currVal = document.getElementById(selectID).value;


	//if the new Division type selected is a nameable div
	if(currVal.indexOf('!')==0)
	{
	  document.getElementById('divNameTableRow'+currID).className='txtNamedDivVisible';

	 var j=i;
	 //Make the name textboxes of all other pages of this div visible
	 while(document.getElementById('selectDivType'+spanArray[j]).disabled==true)
  	 {
	  //alert(i);
	  if(document.getElementById('selectDivType'+ spanArray[j]))
	   { 
	     document.getElementById('selectDivType'+ spanArray[j]).value = currVal;
		 document.getElementById('txtDivName'+spanArray[j]).disabled = true;
	   }
	  if(document.getElementById('divNameTableRow'+ spanArray[j]))
		document.getElementById('divNameTableRow'+ spanArray[j]).className = 'txtNamedDivVisible';
	  j++;
	 }


	}
	
	//else if the division type selected is not a nameable div
	else if(currVal.indexOf('!')==-1)
	{
	 //Hide the name textbox for this page
	 document.getElementById('divNameTableRow'+currID).className='txtNamedDivHidden';
	 var j=i;
	
	//Hide the name textboxes of all the other pages of this division type
    while(document.getElementById('selectDivType'+spanArray[j]).disabled==true)
  	 {
	  //alert(i);
	  
	 if(document.getElementById('selectDivType'+ spanArray[j]))
	    document.getElementById('selectDivType'+ spanArray[j]).value = currVal;
	  if(document.getElementById('divNameTableRow'+ spanArray[j]))
		document.getElementById('divNameTableRow'+ spanArray[j]).className = 'txtNamedDivHidden';
	  j++;
	 }
	}
	

}


//Update the division name through the textboxes of the division type when changed in one
function DivNameTextChanged(textboxID, MaxPageCount)
{
   //Get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	//Get the array of all UI thumbnails from the global variable if previously assigned
	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	 //else set the array values here
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }
     var currVal = document.getElementById(textboxID).value;
     var currID = textboxID.split('txtDivName')[1];
  
  //Update the textboxes of the same division after this one
  var i=spanArray.indexOf(currID)+1;

  while(document.getElementById('selectDivType'+spanArray[i]).disabled==true)
  {
     document.getElementById('txtDivName'+spanArray[i]).value = currVal;
	 i++;
  }
  //If updated somewhere in the middle of the div, also update the previous textboxes all the way till the start of the current division
  if(document.getElementById('selectDivType'+currID).disabled==true)
  { 
    i=spanArray.indexOf(currID)-1;
	while(i>0 && document.getElementById('selectDivType'+spanArray[i]).disabled==true)
	{
	  document.getElementById('txtDivName'+spanArray[i]).value = currVal;
	  i--;
	}
	document.getElementById('txtDivName'+spanArray[i]).value = currVal;
  }
  
  

}


//Autonumber subsequent textboxes on changing one textbox value
function PaginationTextChanged(textboxID,mode,MaxPageCount)
{
    //get the list of all the thumbnail spans on the page
	var spanArrayObjects = new Array();

	if(window.spanArrayGlobal!= null)
	 { 
	  spanArrayObjects = window.spanArrayGlobal;
	 }
	else
	{
	  for(var j=0;j<MaxPageCount;j++)
	  {
	     spanArrayObjects[j]='span'+j;
	  }
	 
	}
	
	 var spanArray=new Array();

	 //get the spanIDs from the array of span objects 
	 for(var k=0;k<spanArrayObjects.length;k++)
	 {
	 //var pageIndex = spanID.split('span')[1];
	   spanArray[k]=spanArrayObjects[k].split('span')[1];
	 }

  //Mode '0': Autonumber all the thumbnail page names till the end
  if(mode=='0')
  {
    var textboxValue = document.getElementById(textboxID).value;
	var lastNumber = textboxValue.split(" ")[(textboxValue.split(" ").length-1)];
	
	var textOnlyLastBox=document.getElementById('Autonumber_text_without_number');
	var numberOnlyLastBox=document.getElementById('Autonumber_number_only');
	
//	lastNumber = lastNumber.toUpperCase().trim();
    var matches = document.getElementById(textboxID).value.match(/\d+/g);
	var varRomanMatches = true;
	var isRomanLower=true;

	for(var x=0;x<lastNumber.length;x++)
	{
	  var c=lastNumber.charAt(x);
	  if("IVXLCDM".indexOf(c)==-1 && "ivxlcdm".indexOf(c)==-1)
	  {
     	  varRomanMatches=false;
	  }

	}

	if (matches != null) 
    {
       // the id attribute contains a digit
       var len=matches.length;
       var number = matches[len-1];
	   var nonNumber='';
	   var val=document.getElementById(textboxID).value;

       
	   //if the number is at the end of the string, with a space before
	   if(val.indexOf(number.toString())==(val.length-number.toString().length) && val.substr(val.indexOf(number.toString())-1,1)==' ')
       {
	       //Set the QC form hidden variable with this mode
		           var hidden_autonumber_mode = document.getElementById('Autonumber_mode');
                   hidden_autonumber_mode.value = '0';
				   
				   var hidden_number_system = document.getElementById('Autonumber_number_system');
				   hidden_number_system.value='decimal';
        
      
			var i;
			var j='';
			var lastIndex=0;
			for(i=spanArray.indexOf(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			{
			  number++;
			 //alert(i);
			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			    lastIndex=i;
				numberOnlyLastBox.value=number.toString();
				textOnlyLastBox.value=document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1);
				j=document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1)+' '+number.toString();
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1)+' '+number.toString();
			  }//end if
			}//end for
          // 	if(i>=spanArray.indexOf(currPageLastIndex))
			
			var hidden_filename=document.getElementById('filename'+spanArray[lastIndex]);
			alert(hidden_filename.value);
			var hidden_filename_from_form = document.getElementById('Autonumber_last_filename');
			hidden_filename_from_form.value=hidden_filename.value;
			
			{alert(j);}
       }//end if
    }//end if
    else if(varRomanMatches==true)
	{
//	   alert('Possible roman numeral detected!');
	   var romanToNumberError="No error";
	   
	   for(var x=0;x<lastNumber.length;x++)
		{
		  var c=lastNumber.charAt(x);
		  if("IVXLCDM".indexOf(c)>-1)
		  {
			  isRomanLower=false;
		  }
		  else
		  {
		     isRomanLower =true;
		  }
		}
//	   alert(isRomanLower);
	   
   
        var roman = lastNumber.toUpperCase().trim();
        
		if(roman.split('V').length>2||roman.split('L').length>2||roman.split('D').length>2)
		{
     		romanToNumberError="Rule 4 violated";
	    }	  
		//Rule 1
		var count=1;
		var last='Z';
		for(var x=0;x<roman.length;x++)
		{
		  //Duplicate?
		  if(roman.charAt(x)==last)
		  {
		    count++;
			if(count==4)
			{
			  romanToNumberError="Rule 1 violated";
			}

		  }
		  else
		  {
			  count=1;
			  last = roman.charAt(x);
		  }
		}

		//Create an arraylist containing the values
		var ptr=0;
		var values = new Array();
		var maxDigit=1000;
        var digit=0;
		var nextDigit=0;
		
		while (ptr<roman.length)
		{
		  //Base value of digit
		  var numeral=roman.charAt(ptr);
		  switch(numeral)
		  {
		   case "I":
		     digit=1;
			 break;
		   case "V":
		     digit=5;
			 break;
		   case "X":
		      digit=10;
			  break;
		   case "L":
			digit=50;
			break;
		   case "C":
		     digit=100;
			 break;
		  case "D":
		      digit=500;
			  break;
		   case "M":
		      digit=1000;
			  break;
		  
		  }
		 //Rule 3
         if(digit>maxDigit)		 
		 {
		   romanToNumberError="Rule 3 violated";
		 }
		 
		 //Next digit
		 var nextDigit=0;
		 if(ptr<roman.length-1)
		 {
		  var nextNumeral=roman.charAt(ptr+1);
		  switch(nextNumeral)
		  {
		   case "I":
		     nextDigit=1;
			 break;
		   case "V":
		     nextDigit=5;
			 break;
		   case "X":
		      nextDigit=10;
			  break;
		   case "L":
			nextDigit=50;
			break;
		   case "C":
		     nextDigit=100;
			 break;
		  case "D":
		      nextDigit=500;
			  break;
		   case "M":
		      nextDigit=1000;
			  break;
		  
		  }
		  if(nextDigit>digit)
		  {
		    if("IXC".indexOf(numeral)==-1 || nextDigit>(digit*10) || roman.split(numeral).length>3)
			  {
			   romanToNumberError="Rule 3 violated";
			  }
			  maxDigit=digit-1;
			  digit=nextDigit-digit;
			  ptr++;
		  }
		  
		 }
		 values.push(digit);
		 //next digit
		 ptr++;
	//	  alert(values);
		
		
		}
		//Rule 5
		for(var i=0;i<values.length-1;i++)
		{
		  if(values[i]<values[i+1])
		  {
		    romanToNumberError="Rule 5 violated";
		  }
		}
		
		//Rule 2
		var total=0;
		for(var i=0;i<values.length;i++)
		{
		  total=total+values[i];
		}
		
        if((typeof total)=="number" && (romanToNumberError=="No error"))
		{
		 // alert(total);
		   //Set the QC form hidden variable with this mode
		   var hidden_autonumber_mode = document.getElementById('Autonumber_mode');
		   hidden_autonumber_mode.value = '0';
		   
		   var hidden_number_system = document.getElementById('Autonumber_number_system');
		   hidden_number_system.value='roman';
		 
		 
		  //Now autonumber all the remaining textboxes of the document
		  for(var i=spanArray.indexOf(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			{
			  total++;
			 //alert(i);
			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			  
			    var number=total;
				
				//Convert decimal "total" back to a roman numeral
				
				//Set up the key-value arrays
				var values=[1000,900,500,400,100,90,50,40,10,9,5,4,1];
				var numerals=["M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"];
				
				//initialize the string
				var result="";
				
				for(var x=0;x<13;x++)
				{
				  //If the number being converted is less than the current key value, append the corresponding numeral or numerical pair to the resultant string
				  while(number>=values[x])
				  {
				    number=number-values[x];
					result=result+numerals[x];
					
				  }
				}
				
//				alert(result);
				if(isRomanLower)
				{
				  result=result.toLowerCase();
				}
				
				//End conversion to roman numeral

//				alert((document.getElementById(textboxID).value.length)-(lastNumber.length)-1);
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,((document.getElementById(textboxID).value.length)-(lastNumber.length))-1)+' '+result;
			  }//end if
			}//end for
		  
		
		}
	}
  }//end if
 
 //Mode '1': Autonumber all the thumbnail pages till the start of the next div
   if(mode=='1')
  {
    var textboxValue = document.getElementById(textboxID).value;
	var lastNumber = textboxValue.split(" ")[(textboxValue.split(" ").length-1)];
//	lastNumber = lastNumber.toUpperCase().trim();
    var matches = document.getElementById(textboxID).value.match(/\d+/g);
	var varRomanMatches = true;
	var isRomanLower=true;

	for(var x=0;x<lastNumber.length;x++)
	{
	  var c=lastNumber.charAt(x);
	  if("IVXLCDM".indexOf(c)==-1 && "ivxlcdm".indexOf(c)==-1)
	  {
     	  varRomanMatches=false;
	  }

	}

    if (matches != null) 
    {
       // the id attribute contains a digit
       var len=matches.length;
       var number = matches[len-1];
	   var nonNumber='';
	   var val=document.getElementById(textboxID).value;
      // alert(number);
       
	   //if the number is at the end of the string, with a space before
	   if(val.indexOf(number.toString())==(val.length-number.toString().length) && val.substr(val.indexOf(number.toString())-1,1)==' ')
       {
		   //Set the QC form hidden variable with this mode
		   var hidden_autonumber_mode = document.getElementById('Autonumber_mode');
		   hidden_autonumber_mode.value = '1';
		   
		   var hidden_number_system = document.getElementById('Autonumber_number_system');
		   hidden_number_system.value='decimal';
	   
      //      for(var i=parseInt(textboxID.split('textbox')[1])+1;i<=MaxPageCount;i++)
			var i=spanArray.indexOf(textboxID.split('textbox')[1])+1;
			while(document.getElementById('selectDivType'+spanArray[i]).disabled==true && i<MaxPageCount)
			{
			  number++;
			 //alert(i);
			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,(document.getElementById(textboxID).value.length-number.toString.length)-1)+' '+number.toString();
				 
//				 if(i==spanArray.indexOf(currPageLastIndex))
//				  alert(i);
			  }//end if
			  i++;
			}//end while
			//if(i>=MaxPageCount)
		//	alert(i);

		  
       }//end if
    }//end if

	//else check for roman numeral autonumbering
	
	 else if(varRomanMatches==true)
	{
//	   alert('Possible roman numeral detected!');
	   var romanToNumberError="No error";
	   
	   for(var x=0;x<lastNumber.length;x++)
		{
		  var c=lastNumber.charAt(x);
		  if("IVXLCDM".indexOf(c)>-1)
		  {
			  isRomanLower=false;
		  }
		  else
		  {
		     isRomanLower =true;
		  }
		}
//	   alert(isRomanLower);
	   
   
        var roman = lastNumber.toUpperCase().trim();
        
		if(roman.split('V').length>2||roman.split('L').length>2||roman.split('D').length>2)
		{
     		romanToNumberError="Rule 4 violated";
	    }	  
		//Rule 1
		var count=1;
		var last='Z';
		for(var x=0;x<roman.length;x++)
		{
		  //Duplicate?
		  if(roman.charAt(x)==last)
		  {
		    count++;
			if(count==4)
			{
			  romanToNumberError="Rule 1 violated";
			}

		  }
		  else
		  {
			  count=1;
			  last = roman.charAt(x);
		  }
		}

		//Create an arraylist containing the values
		var ptr=0;
		var values = new Array();
		var maxDigit=1000;
        var digit=0;
		var nextDigit=0;
		
		while (ptr<roman.length)
		{
		  //Base value of digit
		  var numeral=roman.charAt(ptr);
		  switch(numeral)
		  {
		   case "I":
		     digit=1;
			 break;
		   case "V":
		     digit=5;
			 break;
		   case "X":
		      digit=10;
			  break;
		   case "L":
			digit=50;
			break;
		   case "C":
		     digit=100;
			 break;
		  case "D":
		      digit=500;
			  break;
		   case "M":
		      digit=1000;
			  break;
		  
		  }
		 //Rule 3
         if(digit>maxDigit)		 
		 {
		   romanToNumberError="Rule 3 violated";
		 }
		 
		 //Next digit
		 var nextDigit=0;
		 if(ptr<roman.length-1)
		 {
		  var nextNumeral=roman.charAt(ptr+1);
		  switch(nextNumeral)
		  {
		   case "I":
		     nextDigit=1;
			 break;
		   case "V":
		     nextDigit=5;
			 break;
		   case "X":
		      nextDigit=10;
			  break;
		   case "L":
			nextDigit=50;
			break;
		   case "C":
		     nextDigit=100;
			 break;
		  case "D":
		      nextDigit=500;
			  break;
		   case "M":
		      nextDigit=1000;
			  break;
		  
		  }
		  if(nextDigit>digit)
		  {
		    if("IXC".indexOf(numeral)==-1 || nextDigit>(digit*10) || roman.split(numeral).length>3)
			  {
			   romanToNumberError="Rule 3 violated";
			  }
			  maxDigit=digit-1;
			  digit=nextDigit-digit;
			  ptr++;
		  }
		  
		 }
		 values.push(digit);
		 //next digit
		 ptr++;
	//	  alert(values);
		
		
		}
		//Rule 5
		for(var i=0;i<values.length-1;i++)
		{
		  if(values[i]<values[i+1])
		  {
		    romanToNumberError="Rule 5 violated";
		  }
		}
		
		//Rule 2
		var total=0;
		for(var i=0;i<values.length;i++)
		{
		  total=total+values[i];
		}
		
        if((typeof total)=="number" && (romanToNumberError=="No error"))
		{
		   //Set the QC form hidden variable with this mode
		   var hidden_autonumber_mode = document.getElementById('Autonumber_mode');
		   hidden_autonumber_mode.value = '1';
		   
		   var hidden_number_system = document.getElementById('Autonumber_number_system');
		   hidden_number_system.value='roman';
		   
		
		  //Now autonumber all the remaining textboxes of this div
		  var i=spanArray.indexOf(textboxID.split('textbox')[1])+1;
		  while(document.getElementById('selectDivType'+spanArray[i]).disabled==true && i<MaxPageCount)
			{
			  total++;

			  if(document.getElementById('textbox'+spanArray[i]))
			  {
			  
			    var number=total;
				
				//Convert decimal "total" back to a roman numeral
				
				//Set up the key-value arrays
				var values=[1000,900,500,400,100,90,50,40,10,9,5,4,1];
				var numerals=["M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"];
				
				//initialize the string
				var result="";
				
				for(var x=0;x<13;x++)
				{
				  //If the number being converted is less than the current key value, append the corresponding numeral or numerical pair to the resultant string
				  while(number>=values[x])
				  {
				    number=number-values[x];
					result=result+numerals[x];
					
				  }
				}
				
//				alert(result);
				if(isRomanLower)
				{
				  result=result.toLowerCase();
				}
				
				//End conversion to roman numeral

//				alert((document.getElementById(textboxID).value.length)-(lastNumber.length)-1);
			    document.getElementById('textbox'+spanArray[i]).value = 
				 document.getElementById(textboxID).value.substr(0,((document.getElementById(textboxID).value.length)-(lastNumber.length))-1)+' '+result;
			  }//end if
			  i++;
			}//end for
		  
		
		}
	}
	
	
	
	//end roman numeral auto numbering
	
	

  }//end if
  
  
}//end function


//Assign the 'main thumbnail' to the selected thumbnail span
function PickMainThumbnail(spanID)
{
	var pageIndex = spanID.split('span')[1];
	var hiddenfield = document.getElementById('Main_Thumbnail_Index');
	var hidden_request = document.getElementById('QC_behaviors_request');
	 hidden_request.value="";

	
	//Cursor currently set to the "Pick Main Thumbnail" cursor?
	if($('body').css('cursor').indexOf("thumbnail_cursor")>-1)
	{
	  var spanImageID='spanImg'+pageIndex;
	  //is there a previously selected Main Thumbnail?
	  if(hiddenfield.length>0 && document.getElementById('spanImg'+hiddenfield).className=="pickMainThumbnailIconSelected")
	  {
	    //First unmark the existing one as the main thumbnail
		document.getElementById('spanImg'+hiddenfield).className='pickMainThumbnailIcon';
		
		//Then set the hidden request value to 'unpick'
		hidden_request.value='unpick_main_thumbnail';
							
	  }
	  
	  //User selects a main thumbnail
	  if(document.getElementById(spanImageID).className=="pickMainThumbnailIcon")
	  {
		document.getElementById(spanImageID).className="pickMainThumbnailIconSelected";
		
		//Remove the current cursor class
		$('body').removeClass('qcPickMainThumbnailCursor');
		//Change the cursor back to default
		$('body').addClass('qcResetMouseCursorToDefault');
		
		//Set the hidden field value with the main thumbnail
		hiddenfield.value = pageIndex;
	    hidden_request.value = "pick_main_thumbnail";

		
	  }
	  else
	  {
	  //Confirm if the user wants to unmark this as a thumbnail image
	  
		//   var t=confirm('Are you sure you want to remove this as the main thumbnail?');   
		 // if(t==true)
		  {
			  document.getElementById(spanImageID).className = "pickMainThumbnailIcon";
			 //Change the cursor back to default
			 $('body').addClass('qcResetMouseCursorToDefault');
			 
			 //Set the hidden field value with the main thumbnail
			 
			 hiddenfield.value = pageIndex;
	         hidden_request.value = 'unpick_main_thumbnail';  
			 
			 			 
		  }
	  }
	  // Submit this
	  document.itemNavForm.submit();
	  return false;
	  
	}

}

//Show the QC Icons below the thumbnail on mouseover
function showQcPageIcons(spanID)
{
  //alert(spanID);
  var pageIndex = spanID.split('span')[1];
  var qcPageIconsSpan = 'qcPageOptions'+pageIndex;
  document.getElementById(qcPageIconsSpan).className = "qcPageOptionsSpanHover";
}

//Hide the QC Icon bar below the thumbnail on mouseout
function hideQcPageIcons(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcPageIconsSpan = 'qcPageOptions'+pageIndex;
  document.getElementById(qcPageIconsSpan).className = "qcPageOptionsSpan";
}

//Show the error icon on mouseover
function showErrorIcon(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcErrorIconSpan = 'error'+pageIndex;
  //document.getElementById(qcErrorIconSpan).className = "errorIconSpanHover";
}

//Hide the error icon on mouseout
function hideErrorIcon(spanID)
{
  var pageIndex = spanID.split('span')[1];
  var qcErrorIconSpan = 'error'+pageIndex;
  //document.getElementById(qcErrorIconSpan).className = "errorIconSpan";

}

//Change the cursor to the custom cursor for Selecting a Main Thumbnail
//On clicking on the "Pick Main Thumbnail" icon in the menu bar
function ChangeMouseCursor(MaxPageCount)
{

	//If this cursor is already set, change back to default
	if($('body').css('cursor').indexOf("thumbnail_cursor")>-1)
	{
	//Remove custom cursor classes if any
	$('body').removeClass('qcPickMainThumbnailCursor');
	$('body').removeClass('qcMovePagesCursor');
	$('body').removeClass('qcDeletePagesCursor');

	//Reset to default
	$('body').addClass('qcResetMouseCursorToDefault');
	
	//Clear and hide all the 'move' checkboxes, in case currently visible
	for(var i=0;i<MaxPageCount; i++)
	{
	  if(document.getElementById('chkMoveThumbnail'+i))
	  {
		  document.getElementById('chkMoveThumbnail'+i).checked=false;
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
	  }
	
	}
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	document.getElementByOd('divDeleteMoveOnScroll').className = 'qcDivDeleteButtonHidden';
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}

	}

    else
	{
		//Remove the default cursor style class, and any other custom class first before setting this one, 
		//otherwise it will override the custom cursor class
		$('body').removeClass('qcResetMouseCursorToDefault');
		$('body').removeClass('qcMovePagesCursor');
		$('body').removeClass('qcDeletePagesCursor');
		
		//Set the custom cursor
		$('body').addClass('qcPickMainThumbnailCursor');

			//Clear and hide all the 'move' checkboxes, in case currently visible
			for(var i=0;i<MaxPageCount; i++)
			{
			  if(document.getElementById('chkMoveThumbnail'+i))
			  {
				  document.getElementById('chkMoveThumbnail'+i).checked=false;
				  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
			  }
			
			}

		//Also re-hide the button for moving multiple pages in case previously made visible
		document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
		document.getElementById('divDeleteMoveOnScroll').className = 'qcDivDeleteButtonHidden';
		
		 //Hide all the left/right arrows for moving pages
		for(var i=0; i<MaxPageCount; i++)
		{
				 if(document.getElementById('movePageArrows'+i))
				   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
			
		}
	}	
}

function ResetCursorToDefault(MaxPageCount)
{
	//Remove custom cursor classes if any
	$('body').removeClass('qcPickMainThumbnailCursor');
	$('body').removeClass('qcMovePagesCursor');
	$('body').removeClass('qcDeletePagesCursor');

	//Reset to default
	$('body').addClass('qcResetMouseCursorToDefault');
	
	//Clear and hide all the 'move' checkboxes, in case currently visible
	for(var i=0;i<MaxPageCount; i++)
	{
	  if(document.getElementById('chkMoveThumbnail'+i))
	  {
		  document.getElementById('chkMoveThumbnail'+i).checked=false;
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
	  }
	
	}
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	document.getElementById('divDeleteMoveOnScroll').className = 'qcDivDeleteButtonHidden';
	
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}
}

//Change cursor: move pages
function MovePages(MaxPageCount)
{
//If this cursor is already set, change back to default
if($('body').css('cursor').indexOf("move_pages_cursor")>-1)
{
	//Remove custom cursor classes if any
	$('body').removeClass('qcPickMainThumbnailCursor');
	$('body').removeClass('qcMovePagesCursor');
	$('body').removeClass('qcDeletePagesCursor');

	//Reset to default
	$('body').addClass('qcResetMouseCursorToDefault');
	
	//Clear and hide all the 'move' checkboxes, in case currently visible
	for(var i=0;i<MaxPageCount; i++)
	{
	  if(document.getElementById('chkMoveThumbnail'+i))
	  {
		  document.getElementById('chkMoveThumbnail'+i).checked=false;
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
	  }
	
	}
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButtonHidden';
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}

}
else
{
//Remove the default cursor style class first before setting the custom one, 
//otherwise it will override the custom cursor class
$('body').removeClass('qcResetMouseCursorToDefault');
$('body').removeClass('qcPickMainThumbnailCursor');
$('body').removeClass('qcDeletePagesCursor');

//First change the cursor
$('body').addClass('qcMovePagesCursor');

   //Unhide all the checkboxes
	for(var i=0;i<MaxPageCount; i++)
	{
		if(document.getElementById('chkMoveThumbnail'+i))
		{
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailVisible';
		}
	}

}
}


function DeletePages(MaxPageCount)
{
//Change the mouse cursor, unhide all the checkboxes
//If this cursor is already set, change back to default
if($('body').css('cursor').indexOf("delete_cursor")>-1)
{
	//Remove custom cursor classes if any
	$('body').removeClass('qcPickMainThumbnailCursor');
	$('body').removeClass('qcMovePagesCursor');
	$('body').removeClass('qcDeletePages');

	//Reset to default
	$('body').addClass('qcResetMouseCursorToDefault');
	
	//Clear and hide all the 'move' checkboxes, in case currently visible
	for(var i=0;i<MaxPageCount; i++)
	{
	  if(document.getElementById('chkMoveThumbnail'+i))
	  {
		  document.getElementById('chkMoveThumbnail'+i).checked=false;
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailHidden';
	  }
	
	}
	//Also re-hide the button for moving multiple pages in case previously made visible
	document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
	document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButtonHidden';
	 //Hide all the left/right arrows for moving pages
	for(var i=0; i<MaxPageCount; i++)
	{
			 if(document.getElementById('movePageArrows'+i))
			   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
		
	}

}
else
{
//Remove the default cursor style class first before setting the custom one, 
//otherwise it will override the custom cursor class
$('body').removeClass('qcResetMouseCursorToDefault');
$('body').removeClass('qcPickMainThumbnailCursor');
$('body').removeClass('qcMovePagesCursor');

//First change the cursor
$('body').addClass('qcDeletePagesCursor');

   //Unhide all the checkboxes
	for(var i=0;i<MaxPageCount; i++)
	{
		if(document.getElementById('chkMoveThumbnail'+i))
		{
		  document.getElementById('chkMoveThumbnail'+i).className='chkMoveThumbnailVisible';
		}
	}

}
}



//Make the thumbnails sortable
function MakeSortable1()
{

 var startPosition;
 var newPosition; 
 var oldArray;
 var newArray;

$("#allThumbnailsOuterDiv").sortable({containment: 'parent',
											start: function(event, ui)
                                             {
											   startPosition=$(ui.item).index()+1;
											 },
                                             stop: function(event, ui)
									         {
												   newPosition = $(ui.item).index()+1;
																								  
												
												//get the list of all the thumbnail spans on the page
												 var spanArrayObjects = $(ui.item).parent().children();
												 
												 var spanArray=new Array();
												
												 //get the spanIDs from the array of span objects 
												 for(var i=0;i<spanArrayObjects.length;i++)
												 {
												   spanArray[i]=spanArrayObjects[i].id;
												 }
																								
												//save the array of spans in the UI as a global window variable
												 window.spanArrayGlobal = spanArray;
												
												//if position has been changed, update the page division correspondingly
												  if(startPosition != newPosition)
												  {
												    //get the spanID of the current span being dragged & dropped
													 var spanID=$(ui.item).attr('id');
													 var pageIndex = spanID.split('span')[1];
													
													
												   //get the current index of the moved span in the UI spanArray
													var indexSpanArray = spanArray.indexOf(spanID);   												
													   
													var nextPosition=spanArray[indexSpanArray+1].split('span')[1];
																										
													var indexTemp = spanArray[startPosition].split('span')[1];
													
													//If the span being moved is the start of a new Div 															
													if (document.getElementById('newDivType' + spanArray[newPosition-1].split('span')[1]).checked == true)
													{
													   //alert('Moving a new division page');

                                                        //If the next div is not the start of a new division 
													   if (document.getElementById('newDivType' + (spanArray[startPosition].split('span')[1])).checked == false)
													   {
													        document.getElementById('newDivType' + (spanArray[startPosition].split('span')[1])).checked = true;
													        document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).disabled = false;
													        //alert('still in the right place');
													        document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).value = document.getElementById('selectDivType' + pageIndex).value;

													        //Update the division name textbox
													        if (document.getElementById('selectDivType' + (spanArray[startPosition].split('span')[1])).value.indexOf('!')==0)
													        {
													            document.getElementById('divNameTableRow' + (spanArray[startPosition].split('span')[1])).className = 'txtNamedDivVisible';
													            document.getElementById('txtDivName' + (spanArray[startPosition].split('span')[1])).disabled = false;

													        }
													        else
													        {
													            document.getElementById('txtDivName' + (spanArray[startPosition].split('span')[1])).disabled = true;
													        }
													   }
													    //else do nothing
													}
													//else do nothing
													
													//CASE 1: 
													//If the new position is position 0: Theoretically this cannot happen since the sortable list container boundary
													//set makes this impossible, but just in case.
													if(indexSpanArray==0)
													{
							                          //Make the moved div the start of a new div
													  document.getElementById('newDivType'+(spanArray[newPosition-1].split('span')[1])).checked=true;
													  //Enable the moved div's DivType dropdown
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).disabled=false;
													  //Set the moved div's DivType value to that of the one it is replacing
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).value = document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).value;
													  
													  //Unmark the replaced div's NewDiv Checkbox (and disable the dropdown)
													  document.getElementById('newDivType'+(spanArray[newPosition].split('span')[1])).checked=false;
													  document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).disabled=true;
	                                                  
													  //If this is now a named div, update the division name textbox
													  if(document.getElementById('selectDivType'+(spanArray[newPosition].split('span')[1])).value.IndexOf('!')==0)
													  {
													      document.getElementById('divNameTableRow' + (spanArray[newPosition].split('span')[1])).className = 'txtNamedDivVisible';
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1])).value = document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).value;
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).disabled=true;
													  }
													  else
													  {
													      document.getElementById('divNameTableRow' + (spanArray[newPosition].split('span')[1])).className = 'txtNamedDivHidden';
														document.getElementById('txtDivName'+(spanArray[newPosition].split('span')[1]+1)).disabled=false;
													  }
													}
													
													//else
													//CASE 2: Span moved to any location other than 0
																									
													else if (indexSpanArray > 0)
													{
													 //Moved span's DivType = preceding Div's Div type
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).value = document.getElementById('selectDivType'+(spanArray[newPosition-2].split('span')[1])).value;
													  //Moved span != start of a new Division
													  document.getElementById('newDivType'+(spanArray[newPosition-1].split('span')[1])).checked=false;
													  document.getElementById('selectDivType'+(spanArray[newPosition-1].split('span')[1])).disabled=true;
													  
													  //update the division name textbox
													  if(document.getElementById('selectDivType'+(spanArray[newPosition-2].split('span')[1])).value.indexOf('!')==0)
													  {
													    document.getElementById('divNameTableRow' + (spanArray[newPosition - 1].split('span')[1])).className = 'txtNamedDivVisible';
														document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).disabled=true;
													    document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).value=document.getElementById('txtDivName'+(spanArray[newPosition-2].split('span')[1])).value;
														
													  }
													  else
													  {
													    document.getElementById('divNameTableRow' + (spanArray[newPosition - 1].split('span')[1])).className = 'txtNamedDivHidden';
														document.getElementById('txtDivName'+(spanArray[newPosition-1].split('span')[1])).disabled=false;
													  }
													  
													  
													}//end else if
											 
													 
												 }//end if(startPosition!=newPosition)
											  

											 },placeholder: "ui-state-highlight"});
									 
$("#allThumbnailsOuterDiv").disableSelection();


                                                 															 

}



//Cancel function: set the hidden field(s) accordingly
function behaviors_cancel_form() 
{
	var hiddenfield = document.getElementById('QC_behaviors_request');
	hiddenfield.value = 'cancel';

	
    // Submit this
    document.itemNavForm.submit();
    return false;
}


//Save function: set the hidden field(s) accordingly
function behaviors_save_form() 
{
    var hiddenfield = document.getElementById('QC_behaviors_request');
	hiddenfield.value = 'save';
	
    // Submit this
    document.itemNavForm.submit();
    return false;

}


//Turn On/Off the autosave option
function changeAutoSaveOption()
{
   var linkID = document.getElementById('autosaveLink');
   var hiddenfield = document.getElementById('Autosave_Option');
   var hiddenfield_behavior = document.getElementById('QC_behaviors_request');
    hiddenfield_behavior.value = 'save';

	if(linkID.innerHTML=='Turn Off Autosave')
	{
	  linkID.innerHTML = 'Turn On Autosave';
	  hiddenfield.value = 'false';
//	  alert(hiddenfield.value);
	}
	else
	{
	 linkID.innerHTML = 'Turn Off Autosave';
	 hiddenfield.value = 'true';
	}
    
	//Submit the form
	document.itemNavForm.submit();
    return false;
}

//Called from the main form every three minutes
function qc_auto_save()
{

	jQuery('form').each(function() {
	    var hiddenfield = document.getElementById('QC_behaviors_request');
		hiddenfield.value = 'save';

		var thisURL =window.location.href.toString();
        // For each form on the page, pass the form data via an ajax POST to
        // the save action
        $.ajax({
					url: thisURL,
					data: 'autosave=true&'+jQuery(this).serialize(),
					type: 'POST',
					async: true,
					success: function(data)
					{
					     //Update the time of saving
						  var currdate = new Date();
						  var hours = currdate.getHours();
						  var minutes = currdate.getMinutes();
						  var ampm = hours >= 12 ? 'PM' : 'AM';
						  hours = hours%12;
						  hours = hours?hours:12;
						  hours = hours<10?'0'+hours:hours;
						  minutes=minutes<10?'0'+minutes:minutes;
						  var time = hours+":"+minutes+' '+ampm;
						  
                          var timeToDisplay = "Saved at "+time;
				//		  $("#displayTimeSaved").text(timeToDisplay);
							
							return false;
		 
					}// end successful POST function
				}); // end jQuery ajax call
    }); // end setting up the autosave on every form on the page
}


//When any 'move page' checkbox is is checked/unchecked
function chkMoveThumbnailChanged(chkBoxID, MaxPageCount)
{

  var checked=false;
  document.getElementById('divMoveOnScroll').className='qcDivMoveOnScrollHidden';
  document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButtonHidden';
 //Hide all the left/right arrows for moving pages
for(var i=0; i<MaxPageCount; i++)
{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconHidden';
	
}
 
 
 //If a checkbox has been checked, and the move_thumbnails cursor is currently set
 if (document.getElementById(chkBoxID).checked==true && $('body').css('cursor').indexOf("move_pages_cursor")>-1)
 {
    document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';
    for(var i=0; i<MaxPageCount; i++)
	{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';
	
	}
}
else if(document.getElementById(chkBoxID).checked==true && $('body').css('cursor').indexOf("delete_cursor")>-1)
{
     document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButton';
   
}
else
{ 
  //Check if there is any other checked checkbox on the screen
  for(var i=0; i<MaxPageCount; i++)
  {
    if((document.getElementById('chkMoveThumbnail'+i)) && document.getElementById('chkMoveThumbnail'+i).checked==true)
	{
	  
	  checked = true;
	}
  }
  
  if(checked==true && $('body').css('cursor').indexOf("move_pages_cursor")>-1)
  {
     document.getElementById('divMoveOnScroll').className='qcDivMoveOnScroll';
     //Unhide the left/right arrows for moving pages
     for(var i=0; i<MaxPageCount; i++)
	{
		 if(document.getElementById('movePageArrows'+i))
		   document.getElementById('movePageArrows'+i).className = 'movePageArrowIconVisible';
	
	}
  
  }
  
  else if(checked==true && $('body').css('cursor').indexOf("delete_cursor")>-1)
  {
    document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButton';
  }
  else
  {
     document.getElementById('divDeleteMoveOnScroll').className='qcDivDeleteButtonHidden';
	 document.getElementById('divMoveOnScroll').className = 'qcDivMoveOnScrollHidden';
  }  

}
  
}


// ------------------ Functions for the Move-Selected-Pages Popup Form---------------------//


//Disable\enable the select dropdowns based on the radio button selected
function rbMovePagesChanged(rbValue)
{
  if(rbValue=='After')
    {
	  document.getElementById('selectDestinationPageList1').disabled=false;
	  document.getElementById('selectDestinationPageList2').disabled=true;
	}
	else if(rbValue=='Before')
	{
	  document.getElementById('selectDestinationPageList2').disabled=false;
	  document.getElementById('selectDestinationPageList1').disabled=true;
	}
}


//Update the popup form based on the target page filename and relative position passed in
function update_popup_form(pageID,before_after)
{
  //alert(pageID+before_after);
  if(before_after=='After')
  {
    if(document.getElementById('selectDestinationPageList1'))
	{
	 // alert(before_after);
	  document.getElementById('rbMovePages1').checked=true;
	  document.getElementById('selectDestinationPageList1').disabled=false;
	  document.getElementById('selectDestinationPageList2').disabled=true;	
      //Change the dropdown selected option as well
	  var ddl = document.getElementById('selectDestinationPageList1');
		var opts = ddl.options.length;
		
		for (var i=0; i<opts; i++)
		{
			if (ddl.options[i].text == pageID)
			{
			  ddl.selectedIndex = i;
			}
		}	
	  
	}
  }
  else if(before_after=='Before')
  {
    if(document.getElementById('selectDestinationPageList1'))
	{
	  document.getElementById('rbMovePages2').checked=true;
	  document.getElementById('selectDestinationPageList1').disabled=true;
	  document.getElementById('selectDestinationPageList2').disabled=false;	

      //Change the dropdown selected option as well
	   var ddl = document.getElementById('selectDestinationPageList2');
		var opts = ddl.options.length;
		for (var i=0; i<opts; i++)
		{
			if (ddl.options[i].text == pageID)
			{
				ddl.selectedIndex = i;
				
			}
		}	  
	}
  }
}

//Move the selected pages
function move_pages_submit()
{
   // alert('in function move_pages_submit');
     var hidden_request = document.getElementById('QC_behaviors_request');
	 var hidden_action = document.getElementById('QC_move_relative_position');
	 var hidden_destination = document.getElementById('QC_move_destination');
	 var file_name='';
	 

	 
	 hidden_request.value='move_selected_pages';
     hidden_action.value = '';
	 hidden_destination.value=file_name;
	 
	 //if 'Before' selected, change to corresponding 'After' unless 'Before' 0th option is selected
	 if(document.getElementById('rbMovePages2').checked==true)
	 {
	   if(document.getElementById('selectDestinationPageList2').selectedIndex>0)
	   { 
	     var ddl=document.getElementById('selectDestinationPageList2');
	     var selIndex = ddl.selectedIndex-1;
		 hidden_action.value = 'After';
		 hidden_destination.value = ddl.options[selIndex].value;
	   //  alert(hidden_destination.value);
	   }
	   else
	   {
	     hidden_action.value = 'Before';
		 var ddl=document.getElementById('selectDestinationPageList2');
		 hidden_destination.value = ddl.options[ddl.selectedIndex].value;
		// alert(hidden_destination.value);
	   }
	   
	 }
	 
	 //else assign the selected 'After' values to the hidden variables
	 else
	 {
	   hidden_action.value = 'After';
	   var ddl=document.getElementById('selectDestinationPageList1');
	   var selIndex = ddl.selectedIndex;
	   hidden_destination.value = ddl.options[selIndex].value;
	//   alert(hidden_destination.value);
	 }
	 
	 document.itemNavForm.submit();
	 return false;
}


//--------------------End of Functions for the Move-Selected-Pages Popup Form----------------//


function ImageDeleteClicked(filename) {

    input_box = confirm("Are you sure you want to delete this page and apply all changes up to this point?");

    if (input_box == true) 
	{
        var hidden_request = document.getElementById('QC_behaviors_request');
        var details = document.getElementById('QC_affected_file');

        hidden_request.value = 'delete_page';
        details.value = filename;

        document.itemNavForm.submit();
    }
    return false;
}


function DeleteSelectedPages()
{
	input_box = confirm("Are you sure you want to delete this page and apply all changes up to this point?");
	
	if (input_box == true) 
	{
        var hidden_request = document.getElementById('QC_behaviors_request');
  //      var details = document.getElementById('QC_affected_file');

        hidden_request.value = 'delete_selected_page';
  //      details.value = filename;

        document.itemNavForm.submit();
    }
    return false;
	
}
 
                
// Function to set the full screen mode 
function qc_set_fullscreen() {
    var x = $("#allThumbnailsOuterDiv1").offset().left;
    var y = $("#allThumbnailsOuterDiv1").offset().top;

    var window_height = $(window).height();
    var new_height = window_height - y - 63;

  //  var window_width = $(window).width();
  //  var new_width = window_width - x - 5;
   // alert('y: ' + y + '    window height: ' + window_height + '      new_height: ' + new_height);
    
    $("#allThumbnailsOuterDiv1").height(new_height);
   // $("#allThumbnailsOuterDiv1").width(new_width);
}

