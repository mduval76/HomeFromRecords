import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FormatService {
  formattedArtistName: string = '';
  formattedDetails: string[] = [];
  formattedSubDetails: string[] = [];
  includesSubstringFormatting: boolean = false;

  constructor() { }

  formatArtistName(str: string): string {
    var hyphenSplits = str.split(" - ").map(hyphenSub => hyphenSub.trim()).filter(hyphenSub => hyphenSub.length > 0);
    var ampersandSplits = str.split(" & ").map(ampersandSub => ampersandSub.trim()).filter(ampersandSub => ampersandSub.length > 0);
    var commaSplits = str.split(",").map(commaSub => commaSub.trim()).filter(commaSub => commaSub.length > 0);
    var swapsNeeded = false;

    if (hyphenSplits.length > 1) {
      var hyphenSubs = hyphenSplits.map(hyphenSub => hyphenSub.split(",").map(sub => sub.trim()).filter(sub => sub.length > 0));

      var swaps = [];
      for (let i = 0; i < hyphenSubs.length; i++) {
        if (hyphenSubs[i].length > 1) {
          swaps.push(this.swapSubstrings(hyphenSubs[i][1], hyphenSubs[i][0]));
        } 
        else {
          swaps.push(hyphenSplits[i]);
        }
      }

      if (swaps.length === 2) {
        this.formattedArtistName = swaps.join(" - ");
      } 
      else {
        this.formattedArtistName = swaps.join(", ");
        var lastCommaIndex = this.formattedArtistName.lastIndexOf(",");
        if (lastCommaIndex !== -1) {
          this.formattedArtistName = this.formattedArtistName.substring(0, lastCommaIndex) + " &" + this.formattedArtistName.substring(lastCommaIndex + 1);
        }
      }
    }
    else if (ampersandSplits.length > 1) {

      if (ampersandSplits.length > 1) {
        var swaps = [];
        for (let i = 0; i < ampersandSplits.length; i++) {
          var commaSplits = ampersandSplits[i].split(",").map(sub => sub.trim()).filter(sub => sub.length > 0);
            if (commaSplits.length > 1) {
              swapsNeeded = true;
              swaps.push(this.swapSubstrings(commaSplits[1], commaSplits[0]));
            } 
            else {
              swaps.push(ampersandSplits[i]);
            }
        }
        this.formattedArtistName = swapsNeeded ? swaps.join(" & ") : str;
      } 
      else {
          this.formattedArtistName = str;
      }
    }
    else if(commaSplits.length > 1) {
      if (commaSplits.length === 2) {
        var spaceSplits = commaSplits[1].split(" ").map(spaceSub => spaceSub.trim()).filter(spaceSub => spaceSub.length > 0);

        if (commaSplits.length === 2) {
          var spaceSplits = commaSplits[1].split(" ").map(spaceSub => spaceSub.trim()).filter(spaceSub => spaceSub.length > 0);
          
          if (spaceSplits.length > 2) {
            this.formattedArtistName = this.swapSubstrings(spaceSplits[0], commaSplits[0]);
            this.formattedArtistName += " " + spaceSplits.slice(1).join(" ");
          } 
          else {
            this.formattedArtistName = this.swapSubstrings(commaSplits[1], commaSplits[0]);
          }
        }
      }
      else if (commaSplits.length > 2) {
        if (commaSplits.length === 3) {
          var secondSpaceSplits = commaSplits[1].split(" ").map(spaceSub => spaceSub.trim()).filter(spaceSub => spaceSub.length > 0);
          
          if (secondSpaceSplits.length > 1) {
            this.formattedArtistName = commaSplits[2] + " " + secondSpaceSplits[0] + " " + commaSplits[0] + " " + secondSpaceSplits.slice(1).join(" ");
          } 
          else {
            this.formattedArtistName = str;
          }
        }
      }
      else {
        this.formattedArtistName = str;
      }
    }
    else {
      this.formattedArtistName = str;
    }

    return this.formattedArtistName;
  }

  formatDetails(str: string): { details: string[], subDetails: Map<string, string[]> } {
    let detailsList = str.split(";").map(detail => detail.trim()).filter(detail => detail.length > 0);
    let subDetailsMap = new Map<string, string[]>();
  
    let formattedDetails = detailsList.map(detail => {
      if (detail.startsWith("Includes:")) {
        let { mainDetail, subDetails } = this.formatSubDetails(detail);
        subDetailsMap.set(mainDetail, subDetails);
        return mainDetail;
      }
      return detail;
    });
  
    return { details: formattedDetails, subDetails: subDetailsMap };
  }

  formatSubDetails(detail: string): { mainDetail: string, subDetails: string[] } {
    if (!detail.startsWith("Includes:")) {
      return { mainDetail: detail, subDetails: [] };
    }
  
    this.includesSubstringFormatting = true;
    let subDetails = detail.slice("Includes:".length).trim();
    let formattedSubDetails = subDetails.split(",").map(subDetail => subDetail.trim()).filter(subDetail => subDetail.length > 0);
  
    return { mainDetail: "Includes:", subDetails: formattedSubDetails };
  }

  swapSubstrings(substr1: string, substr2: string): string {
    return substr1 + " " + substr2;
  }
}
