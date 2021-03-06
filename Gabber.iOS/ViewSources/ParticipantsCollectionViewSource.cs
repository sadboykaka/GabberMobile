﻿using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using GabberPCL;
using GabberPCL.Models;
using Gabber.iOS.Helpers;

namespace Gabber.iOS.ViewSources
{
    public class ParticipantsCollectionViewSource : UICollectionViewSource
    {
        public Action<int> AddParticipant;
        public List<User> Rows { get; set; }

        public ParticipantsCollectionViewSource(List<User> _rows)
        {
            Rows = _rows;
        }

        public override nint NumberOfSections(UICollectionView collectionView) 
        {
            // There does not seem to be a way to enable this within storyboard.
            collectionView.AllowsMultipleSelection = true;
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Rows.Count;
        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ToggleSelected(collectionView, indexPath);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ToggleSelected(collectionView, indexPath);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ParticipantsCollectionViewCell)collectionView.DequeueReusableCell(ParticipantsCollectionViewCell.CellID, indexPath);                
            cell.Layer.BorderWidth = 1.0f;
            cell.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            cell.UpdateContent(Rows[indexPath.Row]);
            return cell;
        }

        // Helper method to simplify shared logic between
        void ToggleSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Rows[indexPath.Row].Selected = !Rows[indexPath.Row].Selected;
            Session.Connection.Update(Rows[indexPath.Row]);
            collectionView.ReloadData();
            // Updates the label once its been selected
            AddParticipant(Rows.FindAll((obj) => obj.Selected).Count);

            NSString[] keys = {
                new NSString("NAME"),
                new NSString("EMAIL"),
                new NSString("STATE"),
                new NSString("NUM_PARTICIPANTS"),
                new NSString("TIMESTAMP")
            };
            NSObject[] values = {
                new NSString(Rows[indexPath.Row].Name),
                new NSString(Rows[indexPath.Row].Email),
                new NSString(Rows[indexPath.Row].Selected.ToString()),
                new NSString(Rows.Count.ToString()),
                new NSString(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
            Logger.LOG_EVENT_WITH_DICT("PARTICIPANT_SELECTED", parameters);
        }
    }
}
