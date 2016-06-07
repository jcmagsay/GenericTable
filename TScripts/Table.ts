/// <reference path="../Utilities.ts" />

module TenFour.CommonContent.Components {
	export class Table {
		tableId: string;
        dataList: KnockoutObservableArray;
        hiddenColumns: KnockoutObservableArray;
		allHidden: KnockoutObservableBool;


		constructor(private id: string, private filter: App.Filter.IKnockoutObservablePagedGenericFilter) {
			this.tableId = "#" + id;

            this.dataList = ko.observableArray([]);
		    this.hiddenColumns = ko.observableArray([]);
			this.allHidden = ko.observable(false);

            this.initTableBindings();
            this.applySortToTable(ko.toJS(filter));

		    this.dataList.subscribe(() => { console.info("data-list has changed") });
		}

		initTableBindings() {
			var $tableRoot = $(this.tableId);
			$("tbody", $tableRoot).on("click", "tr", (el) => {
				var target = $(el.target);
				if (!$(target).hasClass("clickable") && !($(target).is("th")) && !($(target).closest("tr").hasClass("active"))) {
					this.rowClick(target);
				}
			});

			$("thead", $tableRoot).on("click", "th", (event) => {
				event.stopImmediatePropagation();
				this.sortColumn(event);
			});
		}

		getParameterByName(name) {
			
		}

		searchAssets(event: JQueryEventObject, clearAll: boolean = false) {
			//alert("search");
		}

		rowClick(target: JQuery) {
			//alert("rowClick");
		}

		sortColumn(event: JQueryEventObject) {
			var target = $(event.currentTarget),
				sort = target.data("sort"),
				sortdir = target.data("sortdir"),
				isActive = target.hasClass("is-active"),
				direction = this.determineSortDir(target, sortdir, isActive);

			var newDirection = direction ? App.Filter.SortDirection.Ascending : App.Filter.SortDirection.Descending;
			
			var sortClause = new App.Filter.SortClause(sort, newDirection);
			this.filter().sorts([sortClause]);
            this.addSortCaret(target, direction);
		}

        private applySortToTable(filter: App.Filter.IPagedGenericFilter) {
            if (filter == null || filter.sorts == null || filter.sorts[0] == null || filter.sorts[0].propertyName == null ) {
                return;
            }
            var sortPropertyName = filter.sorts[0].propertyName;
            var sortDirection = filter.sorts[0].sortDirection === App.Filter.SortDirection.Ascending;
            var $target = $("[data-sort='" + sortPropertyName + "']", this.tableId);
            this.determineSortDir($target, sortDirection, false);
            this.addSortCaret($target, sortDirection);
        }

		replaceBody(bodyContent: JQuery) {
			$("tbody", $(this.tableId)).replaceWith(bodyContent);
		}

		populateData(data: Array<any>) {
			this.dataList(data);
		}

		determineSortDir(target: JQuery, sortDir: boolean, isActive: boolean): boolean {
			var dir: boolean; 
			if (isActive) {
				dir = !(sortDir);
			} else {
				dir = true;
			}
			
			target.closest("tr").find("th").removeClass("is-active");
			target.addClass("is-active");
			target.data("sortdir", dir);
			return dir;
		}

        hideColumn(col: string, isHidden: boolean, target: JQuery) {
			if (isHidden) {
                target.removeClass("is-checked");
			    this.hiddenColumns.push(col);
			} else {
                target.addClass("is-checked");
                this.hiddenColumns.remove(col);
			}

			var labels = $(".mdtf-hybrid__table-control .mdtf-dialog label");
			var allChecked = labels.not(".is-checked").length === labels.length;
			(allChecked) ? this.allHidden(true) : this.allHidden(false);
        }

        isHidden(col: string) {
            return this.hiddenColumns.indexOf(col) >= 0;
        }

        private addSortCaret(target: JQuery, direction: boolean) {
            var $sortCaret = $("<i>");
            var textAlign = target.css("text-align");
            $sortCaret.addClass("material-icons sort-icon").text("arrow_drop_" + (direction ? "up" : "down"));

            $(".sort-icon", this.tableId).remove();
            if (textAlign === "right") {
                target.prepend($sortCaret);
            } else {
                target.append($sortCaret);
            }
         
        }

	}
}