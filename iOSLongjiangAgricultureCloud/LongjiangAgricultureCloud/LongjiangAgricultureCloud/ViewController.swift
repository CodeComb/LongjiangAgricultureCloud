//
//  ViewController.swift
//  LongjiangAgricultureCloud
//
//  Created by Amamiya Yuuko on 15/6/21.
//  Copyright (c) 2015年 龙江县超越现代玉米种植农民专业合作社. All rights reserved.
//

import UIKit

class ViewController: UIViewController, UIWebViewDelegate {

    @IBOutlet weak var webView: UIWebView!
    override func viewDidLoad() {
        super.viewDidLoad()
        let url = NSURL(string: "http://221.208.208.32:7532/Mobile")
        let request = NSURLRequest(URL: url!)
        for subView in webView.subviews
        {
            if (subView.isKindOfClass(UIScrollView))
            {
                (subView as! UIScrollView).bounces = false
                for shadowView in subView.subviews
                {
                    if (shadowView.isKindOfClass(UIImageView))
                    {
                        (shadowView as! UIImageView).hidden = true
                    }
                }
            }
            if (subView.isKindOfClass(UIImageView))
            {
                (subView as! UIImageView).hidden = true
            }
        }
        webView.layer.borderWidth = 0
        webView.opaque = false;
        webView.backgroundColor = UIColor.clearColor()
        webView.scrollView.bounces = false
        webView.delegate = self
        webView.dataDetectorTypes = UIDataDetectorTypes.All
        webView.loadRequest(request)
        // Do any additional setup after loading the view, typically from a nib.
    }
    
    func webView(webView: UIWebView, shouldStartLoadWithRequest request: NSURLRequest, navigationType: UIWebViewNavigationType) -> Bool {
        if (request.URL?.scheme == "tel")
        {
            let urlstr = (request.URL?.absoluteString)!.stringByReplacingOccurrencesOfString("tel:", withString:"tel://")
            let url = NSURL(string: urlstr);
            let result = UIApplication.sharedApplication().openURL(url!)
            return false;
        }
        else if (request.URL?.scheme == "wxpay" || request.URL?.scheme == "alipay")
        {
            let url = request.URL
            let result = UIApplication.sharedApplication().openURL(url!)
            return false;
        }
        return true;
    }
        
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
}

